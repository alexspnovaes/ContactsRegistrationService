using Contacts.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactsRegistrationService.Api.Services;
using ContactsRegistrationService.Api.Controllers;
using Assert = Xunit.Assert;

namespace ContactsRegistrationService.API.Tests.Controllers
{
    public class ContactsControllerTests
    {
        [Fact]
        public async Task CreateContact_ValidData_ReturnsAccepted()
        {
            // Arrange: cria um ContactDTO válido
            var contactDto = new ContactDTO
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Ddd = "123",
                PhoneNumber = "12345678",
                Email = "john.doe@example.com"
            };

            // Cria um mock para a interface IServiceBusPublisher
            var publisherMock = new Mock<IServiceBusPublisher>();
            publisherMock
                .Setup(p => p.PublishContactAsync(It.IsAny<ContactDTO>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Cria um mock para o logger
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ContactsController>>();

            // Instancia o controller, injetando a interface
            var controller = new ContactsController(publisherMock.Object, mockLogger.Object);

            // Act: chama o método CreateContact (supondo que seja assíncrono)
            var result = await controller.CreateContact(contactDto);

            // Assert: espera que o resultado seja Accepted
            var acceptedResult = Assert.IsType<AcceptedResult>(result);
            publisherMock.Verify(p => p.PublishContactAsync(
                It.Is<ContactDTO>(c => c.Id == contactDto.Id)), Times.Once);
        }

        [Fact]
        public async Task CreateContact_NullData_ReturnsBadRequest()
        {
            // Arrange: cria mocks para a interface e logger
            var publisherMock = new Mock<IServiceBusPublisher>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ContactsController>>();
            var controller = new ContactsController(publisherMock.Object, mockLogger.Object);

            // Act: chama CreateContact com null
            var result = await controller.CreateContact(null);

            // Assert: espera que o resultado seja BadRequest
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
