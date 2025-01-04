using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;
        [Required]
        public required string Password { get; set; } = string.Empty;
    }
}
