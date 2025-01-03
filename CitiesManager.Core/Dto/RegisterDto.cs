using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Dto
{
    public class RegisterDto
    {
        [Required]
        public required string PersonName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Remote("IsEmailAlreadyRegistered", "Accounts")]
        public required string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public required string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public required string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public required string ConfirmPassword { get; set; } = string.Empty;
    }
}
