using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Contacts.Shared.Models;

namespace ContactsRegistrationService.Api.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(ServiceBusSender sender)
        {
            _sender = sender;
        }

        public async Task PublishContactAsync(ContactDTO contact)
        {
            var json = JsonSerializer.Serialize(contact);
            var message = new ServiceBusMessage(json);
            await _sender.SendMessageAsync(message);
        }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
