using CitiesManager.Core.Dto;
using CitiesManager.Core.Identities;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponseDto CreateJwtToken(ApplicationUser applicationUser)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id.ToString()), // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT unique ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued at
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Email?.ToString() ?? "") // Unique identifier of user
            ];

            string? jwtSecret = _configuration["Jwt:Key"]; // Need to read from environment variables for real projects
            if (jwtSecret == null)
            {
                throw new NullReferenceException(nameof(jwtSecret));
            }
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtSecret));

            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            string? issuer = _configuration["Jwt:Issuer"];
            string? audience = _configuration["Jwt:Audience"];
            if (issuer == null || audience == null)
            {
                throw new NullReferenceException(nameof(issuer));
                throw new NullReferenceException(nameof(audience));
            }
            JwtSecurityToken securityToken = new(issuer, audience, claims, expires: expiration, signingCredentials: signingCredentials);

            JwtSecurityTokenHandler tokenHandler = new();
            string tokenValueString = tokenHandler.WriteToken(securityToken);

            return new AuthenticationResponseDto()
            {
                Email = applicationUser.Email ?? "",
                PersonName = applicationUser.PersonName,
                Token = tokenValueString,
                Expiration = expiration,
            };
        }
    }
}
