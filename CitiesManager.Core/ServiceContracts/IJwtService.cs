using CitiesManager.Core.Dto;
using CitiesManager.Core.Identities;
using System.Security.Claims;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponseDto CreateJwtToken(ApplicationUser applicationUser);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? jwtToken);
    }
}
