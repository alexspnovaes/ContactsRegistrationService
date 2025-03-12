using System.Text;
using System.Text.Json;
using Contacts.Shared.Models; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ContactsPersistenceService.Consumer.Services
{
    public class RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger, IContactService contactService) : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerService> _logger = logger;
        private readonly IContactService _contactService = contactService;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string QueueName = "contacts_queue";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            _logger.LogInformation("RabbitMQ listener initialized and queue '{QueueName}' declared.", QueueName);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var contact = JsonSerializer.Deserialize<ContactDTO>(json);

                    if (contact != null)
                    {
                        _logger.LogInformation("Received contact: {Name}", contact.Name);
                        await _contactService.CreateContactAsync(contact);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.LogInformation("Contact persisted successfully: {Name}", contact.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Received message does not contain a valid contact.");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message.");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);

            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public async ValueTask DisposeAsync()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            await Task.CompletedTask;
        }
    }
}
