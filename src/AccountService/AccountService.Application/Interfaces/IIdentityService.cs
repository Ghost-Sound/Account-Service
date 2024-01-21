using AccountService.Domain.Entity;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AccountService.Application.Models;

namespace AccountService.Application.Interfaces
{
    public interface IIdentityService
    {
        AuthenticationProperties? CreateAuthenticationProperties(bool rememberMe);
        IdentityServerUser? CreateIdentityServerUserRegister(UserRegistryDTO user, List<Claim> claims);

        IdentityServerUser? CreateIdentityServerUser(User user, List<Claim> claims);

        string CreateUniqueId(CryptoRandom.OutputFormat format = CryptoRandom.OutputFormat.Hex);
    }
}