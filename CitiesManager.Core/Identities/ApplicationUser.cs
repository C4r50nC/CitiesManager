using Microsoft.AspNetCore.Identity;

namespace CitiesManager.Core.Identities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string PersonName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}
