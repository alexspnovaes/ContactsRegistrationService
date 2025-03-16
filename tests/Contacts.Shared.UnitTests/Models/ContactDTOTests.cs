using System.ComponentModel.DataAnnotations;
using Contacts.Shared.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace Contacts.Shared.UnitTests.Models
{
    public class ContactDTOTests
    {
        [Fact]
        public void ContactDTO_InvalidData_Should_FailValidation()
        {
            // Arrange: Dados inválidos
            var model = new ContactDTO
            {
                Id = Guid.NewGuid(),            // Id válido, mas é obrigatório e não há problema
                Name = "Jo",                    // Nome muito curto (mínimo 3 caracteres)
                Ddd = "12",                     // DDD deve ter exatamente 3 dígitos
                PhoneNumber = "1234567",        // Telefone deve ter 8 ou 9 dígitos
                Email = "invalid-email"         // Email com formato inválido
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act: Tenta validar o objeto
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert: A validação deve falhar e conter mensagens específicas
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("between 3 and 50"));  // Nome
            Assert.Contains(results, r => r.ErrorMessage.Contains("exactly 3 digits"));   // DDD
            Assert.Contains(results, r => r.ErrorMessage.Contains("contain only numbers")); // Telefone
            Assert.Contains(results, r => r.ErrorMessage.Contains("Invalid email format")); // Email
        }

        [Fact]
        public void ContactDTO_ValidData_Should_PassValidation()
        {
            // Arrange: Dados válidos
            var model = new ContactDTO
            {
                Id = Guid.NewGuid(),
                Name = "Alice Smith",
                Ddd = "123",
                PhoneNumber = "98765432",
                Email = "alice.smith@example.com"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act: Tenta validar o objeto
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert: A validação deve ser bem-sucedida e não gerar erros
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
