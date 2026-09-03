using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Event_parking.Configurations;
using Event_parking.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Event_parking.Helpers
{
    public class JwtHelper
    {
        private readonly JwtSettings _jwtSettings;

        public JwtHelper(
            IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public (string Token, DateTime ExpiresAt)
            GenerateToken(Customer customer)
        {
            DateTime expiresAt =
                DateTime.UtcNow.AddMinutes(
                    _jwtSettings.ExpiryMinutes
                );

            Claim[] claims =
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    customer.CustomerId.ToString()
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    customer.CustomerId.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    customer.FullName
                ),

                new Claim(
                    ClaimTypes.Email,
                    customer.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    customer.Role
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _jwtSettings.Key
                    )
                );

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );

            var securityToken =
                new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: credentials
                );

            string token =
                new JwtSecurityTokenHandler()
                    .WriteToken(securityToken);

            return (token, expiresAt);
        }
    }
}