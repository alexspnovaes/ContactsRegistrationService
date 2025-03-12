using Contacts.Shared.Models;
using System.Threading.Tasks;

namespace ContactsPersistenceService.Consumer.Services
{
    public interface IContactService
    {
        Task<ContactDTO> CreateContactAsync(ContactDTO contactDto);
    }
}
