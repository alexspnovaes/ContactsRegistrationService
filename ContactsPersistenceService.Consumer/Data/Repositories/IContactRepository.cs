using Contacts.Shared.Models;
using ContactsPersistenceService.Consumer.Data.Entities;
using System.Threading.Tasks;

namespace ContactsPersistenceService.Consumer.Data.Repositories
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
    }
}
