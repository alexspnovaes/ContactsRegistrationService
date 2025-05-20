using Contacts.Shared.Models;

namespace ContactsRegistrationService.Api.Services
{
    public interface IServiceBusPublisher
    {
        Task PublishContactAsync(ContactDTO contact);
    }
}
