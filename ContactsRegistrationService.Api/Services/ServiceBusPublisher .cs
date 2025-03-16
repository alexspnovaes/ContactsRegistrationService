using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Contacts.Shared.Models;
using RabbitMQ.Client;

namespace ContactsRegistrationService.Api.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private const string QueueName = "contacts-queue";

        public ServiceBusPublisher(ServiceBusClient client, ServiceBusSender sender)
        {
            _client = client;
            _sender = sender;
        }

        public static ServiceBusPublisher Create(string connectionString)
        {
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(QueueName);
            return new ServiceBusPublisher(client, sender);
        }

        public async Task PublishContactAsync(ContactDTO contact)
        {
            var json = JsonSerializer.Serialize(contact);
            var message = new ServiceBusMessage(json);
            await _sender.SendMessageAsync(message);
        }

        public async ValueTask DisposeAsync()
        {
            if (_sender != null)
            {
                await _sender.DisposeAsync();
            }
            if (_client != null)
            {
                await _client.DisposeAsync();
            }
        }
    }
}
