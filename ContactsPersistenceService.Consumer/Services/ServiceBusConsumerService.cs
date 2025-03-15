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
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace ContactsPersistenceService.Consumer.Services
{
    public class ServiceBusConsumerService(ILogger<ServiceBusConsumerService> logger,
                                      IContactService contactService,
                                      IConfiguration configuration) : BackgroundService
    {
        private readonly ILogger<ServiceBusConsumerService> _logger = logger;
        private readonly IContactService _contactService = contactService;
        private readonly string _serviceBusConnectionString = configuration.GetConnectionString("ServiceBusConnection") ?? throw new ArgumentNullException(nameof(configuration), "ServiceBusConnection string is null");
        private readonly string _queueName = configuration["ServiceBusQueueName"] ?? "contacts-queue";
        private ServiceBusClient? _client;
        private ServiceBusProcessor? _processor;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new ServiceBusClient(_serviceBusConnectionString);

            _processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
            _logger.LogInformation("Service Bus processor started.");
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            _logger.LogInformation($"Received message: {body}");

            try
            {
                var contact = JsonSerializer.Deserialize<ContactDTO>(body);
                if (contact != null)
                {
                    await _contactService.CreateContactAsync(contact);
                    await args.CompleteMessageAsync(args.Message);
                    _logger.LogInformation("Message processed and completed.");
                }
                else
                {
                    _logger.LogWarning("Message deserialization returned null.");
                    await args.AbandonMessageAsync(args.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"Message handler encountered an exception. Error Source: {args.ErrorSource}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }
            if (_client != null)
            {
                await _client.DisposeAsync();
            }
            await base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
