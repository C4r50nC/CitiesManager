using CitiesManager.Core.Dto;
using CitiesManager.Core.Identities;

namespace CitiesManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponseDto CreateJwtToken(ApplicationUser applicationUser);
    }
}
