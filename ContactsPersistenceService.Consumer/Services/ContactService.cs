using Contacts.Shared.Models;
using ContactsPersistenceService.Consumer.Data.Entities;
using ContactsPersistenceService.Consumer.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsPersistenceService.Consumer.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ContactDTO> CreateContactAsync(ContactDTO contactDto)
        {
            var contact = new Contact
            {
                Name = contactDto.Name,
                PhoneNumber = contactDto.PhoneNumber,
                Email = contactDto.Email,
                Ddd = contactDto.Ddd
            };

            await _contactRepository.AddAsync(contact);
            return contactDto;
        }
    }
}
