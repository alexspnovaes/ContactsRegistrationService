using Contacts.Shared.Models;
using ContactsRegistrationService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ContactsRegistrationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IServiceBusPublisher _publisher;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IServiceBusPublisher publisher, ILogger<ContactsController> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Register new contact",
            Description = "Publishes the contact data to RabbitMQ for asynchronous processing."
        )]
        [SwaggerResponse(202, "Contact accepted for processing.")]
        [SwaggerResponse(400, "Invalid or null data.")]
        public async Task<IActionResult> CreateContact([FromBody] ContactDTO contact)
        {
            if (contact == null)
            {
                _logger.LogWarning("Attempted registration with null data.");
                return BadRequest("Contact cannot be null.");
            }

            await _publisher.PublishContactAsync(contact);
            _logger.LogInformation("Contact {Name} sent to the queue.", contact.Name);
            return Accepted(); 
        }

    }
}
