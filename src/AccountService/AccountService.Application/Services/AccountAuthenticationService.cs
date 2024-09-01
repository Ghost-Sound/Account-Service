using AccountService.Application.Interfaces;
using AccountService.Domain.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CustomHelper.Exception;
using Duende.IdentityServer;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Enums;
using IdentityModel.Client;
using IdentityServerOptions = AccountService.Application.Options.IdentityServerOptions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Services
{
    public class AccountAuthenticationService(
        UserManager<User> userManager,
        IIdentityService identityService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptions<IdentityServerOptions> identityServerOptions) : IAccountAuthenticationService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IIdentityService _identityService = identityService;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IdentityServerOptions _identityServerOptions = identityServerOptions.Value;

        public async Task<(string,string, string)> Login(UserLoginDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    throw new CustomException(message: typeof(UserLoginDTO).FullName!, user);
                }

                //if (user.EmailConfirmed == false)
                //{
                //    throw new CustomException(message: "User not confirmed email", user);
                //}
                var httpClient = _httpClientFactory.CreateClient();

                var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                {
                    Address = _identityServerOptions.URL,
                    Policy = new DiscoveryPolicy
                    {
                        RequireHttps = false
                    }
                });
                if (discoveryDocument.IsError)
                {
                    throw new CustomException("Failed to discover Identity Server", discoveryDocument.Error);
                }

                var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = _identityServerOptions.ClientId,
                    ClientSecret = _identityServerOptions.ClientSecret,
                    Scope = "openid profile offline_access UserManagement GroupManagment role",
                    UserName = user.UserName, //Unique at database
                    Password = model.Password,
                });

                if (tokenResponse.IsError)
                {
                    throw new CustomException(tokenResponse.Error!);
                }

                await _httpContextAccessor.HttpContext.SignInAsync(GetIsuser(user), GetProperties(model.RememberLogin));

                var token = (tokenResponse.AccessToken, tokenResponse.RefreshToken, user.Id.ToString());
                // Return TokenResponse containing Access Token and Refresh Token
                return token!;
            }
            catch
            {
                throw;
            }
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
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            return _identityService.CreateIdentityServerUser(user, claims);
        }
    }
}
