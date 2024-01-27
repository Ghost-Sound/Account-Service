using AccountService.Application.Interfaces;
using AccountService.Domain.Entity;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CustomHelper.Exception;
using Duende.IdentityServer;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Enums;
using Duende.IdentityServer.Configuration;
using IdentityModel.Client;
using IdentityServerOptions = AccountService.Application.Options.IdentityServerOptions;
using AccountService.Application.Options;
using System.Threading;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.Models;

namespace AccountService.Application.Services
{
    public class AuthenticationService : Interfaces.IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityServerOptions _identityServerOptions;

        public AuthenticationService(
            UserManager<User> userManager,
            IIdentityService identityService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityServerOptions> identityServerOptions)
        {
            _userManager = userManager;
            _identityService = identityService;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _identityServerOptions = identityServerOptions.Value;
        }

        public async Task<TokenResponse> Login(UserLoginDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    throw new CustomException(message: typeof(UserLoginDTO).FullName, user);
                }

                //if (user.EmailConfirmed == false)
                //{
                //    throw new CustomException(message: "User not confirmed email", user);
                //}

                var httpClient = _httpClientFactory.CreateClient();

                var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(_identityServerOptions.URL);
                if (discoveryDocument.IsError)
                {
                    throw new CustomException("Failed to discover Identity Server");
                }

                var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = _identityServerOptions.ClientId,
                    ClientSecret = _identityServerOptions.ClientSecret,
                    Scope = "openid profile offline_access UserManagement role",
                    UserName = model.Email,
                    Password = model.Password
                });

                if (tokenResponse.IsError)
                {
                    throw new CustomException(tokenResponse.Error);
                }

                await _httpContextAccessor.HttpContext.SignInAsync(GetIsuser(user), GetProperties(model.RememberLogin));

                // Return TokenResponse containing Access Token and Refresh Token
                return tokenResponse;
            }
            catch
            {
                throw;
            }
        }

        private async Task AddClaimsAndRolesToUser(User user)
        {
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, UserRoles.Student.ToString()));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            await _userManager.AddToRoleAsync(user, UserRoles.Student.ToString());
        }

        private AuthenticationProperties? GetProperties(bool rememberMe)
        {
            // only set explicit expiration here if user chooses "remember me". 
            // otherwise we rely upon expiration configured in cookie middleware.
            return _identityService.CreateAuthenticationProperties(rememberMe);
        }

        private IdentityServerUser? GetIsuser(User user)
        {
            // issue authentication cookie with subject ID and username
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, UserRoles.Student.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            return _identityService.CreateIdentityServerUser(user, claims);
        }
    }
}
