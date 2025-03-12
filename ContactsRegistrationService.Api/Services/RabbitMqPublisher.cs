using System.Text;
using System.Text.Json;
using Contacts.Shared.Models;
using RabbitMQ.Client;

namespace ContactsRegistrationService.Api.Services
{
    public class RabbitMqPublisher : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private const string QueueName = "contacts_queue";

        private RabbitMqPublisher(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        public static async Task<RabbitMqPublisher> CreateAsync()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            return new RabbitMqPublisher(connection, channel);
        }

        public async Task PublishContactAsync(ContactDTO contact)
        {
            var json = JsonSerializer.Serialize(contact);
            var body = Encoding.UTF8.GetBytes(json);
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                body: body);
        }

        public async ValueTask DisposeAsync()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            await Task.CompletedTask;
        }
    }
}
