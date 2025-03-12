using System.ComponentModel.DataAnnotations;

namespace Contacts.Shared.Models
{
    public class ContactDTO
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "DDD is required")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "DDD must be exactly 3 digits")]
        public required string Ddd { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"^\d{8,9}$", ErrorMessage = "PhoneNumber must contain only numbers and be 8 or 9 digits")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public required string Email { get; set; }
    }
}
