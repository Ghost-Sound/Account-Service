using AccountService.Application.Interfaces;
using AccountService.Application.Models.Users;
using AccountService.Application.Options;
using AccountService.Domain.Entity;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AccountService.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public IdentityService() { }
        public string CreateUniqueId(CryptoRandom.OutputFormat format = CryptoRandom.OutputFormat.Hex)
        {
            return CryptoRandom.CreateUniqueId(format: format);
        }

        public AuthenticationProperties? CreateAuthenticationProperties(bool rememberMe)
        {
            AuthenticationProperties? properties = rememberMe ? new AuthenticationProperties()
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
            } : null;

            return properties;
        }

        public IdentityServerUser? CreateIdentityServerUserRegister(UserRegistryDTO user, List<Claim> claims)
        {
            string sub = CreateUniqueId(CryptoRandom.OutputFormat.Hex);

            claims = claims ?? new List<Claim>(); 

            if (!string.IsNullOrEmpty(user.Username))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            return new IdentityServerUser(sub)
            {
                DisplayName = user.Email,
                AdditionalClaims = claims,
            };
        }

        public IdentityServerUser? CreateIdentityServerUser(User user, List<Claim> claims)
        {
            string sub = CreateUniqueId(CryptoRandom.OutputFormat.Hex);

            claims = claims ?? new List<Claim>();

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            return new IdentityServerUser(sub)
            {
                DisplayName = user.Email,
                AdditionalClaims = claims,
            };
        }
    }
}
