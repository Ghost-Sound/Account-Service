using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AccountService.Pages;
using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountService.Application.Models;
using Microsoft.AspNetCore.Identity;
using Duende.IdentityServer.Test;
using IdentityModel;
using System.Security.Claims;
using static Duende.IdentityServer.Models.IdentityResources;
using System.Xml.Linq;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Duende.IdentityServer.Events;
using AccountService.Application.Options;
using Duende.IdentityServer.EntityFramework.Stores;
using Duende.IdentityServer.Validation;
using IdentityModel.Client;
using RefreshTokenRequest = IdentityModel.Client.RefreshTokenRequest;
using CustomHelper.Authentication.Interfaces;

namespace AccountService.API.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISignInKeys _signInKeys;

        public AccountController(
            UserDbContext userDbContext,
            IIdentityServerInteractionService identityServer,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            IEventService events,
            IClientStore clientStore,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ISignInKeys signInKeys)
        {
            _dbContext = userDbContext;
            _interaction = identityServer;
            _signInManager = signInManager;
            _userManager = userManager;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;
            _clientStore = clientStore;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _signInKeys = signInKeys;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO.UserLoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(model);
            }

            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(model);
            }

            if (user.EmailConfirmed == false)
            {
                return BadRequest(new { message = "User not confirmed email", errors = ModelState });
            }

            var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, sub, user.UserName, clientId: context?.Client.ClientId));

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return BadRequest(result.Succeeded);
            }

            // only set explicit expiration here if user chooses "remember me". 
            // otherwise we rely upon expiration configured in cookie middleware.
            AuthenticationProperties props = null;
            if (LoginOptions.AllowRememberLogin && model.RememberLogin)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                };
            };

            // issue authentication cookie with subject ID and username
            var isuser = new IdentityServerUser(sub)
            {
                DisplayName = user.UserName
            };

            await HttpContext.SignInAsync(isuser, props);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserDTO.UserRegistryDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(user.ReturnUrl);

            // the user clicked the "cancel" button
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
            }

            //var user = _users.CreateUser(Input.Username, Input.Password, Input.Name, Input.Email);
            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                return Conflict(user);
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

            var claims = new List<Claim>();
            if (!String.IsNullOrEmpty(user.Username))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
            }
            if (!String.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            var isuser = new IdentityServerUser(sub)
            {
                DisplayName = user.Username
            };

            await HttpContext.SignInAsync(isuser);

            var url = Url.Action(nameof(Register), new { id = 1 }) ?? $"/{1}";

            return Created(url, user);
        }

        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
        {
            var result = await _interaction.GetAuthorizationContextAsync(model.RefreshToken);
            if (result == null)
            {
                return BadRequest("Invalid refresh token");
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(result.Client.ClientId);
            if (client == null)
            {
                return BadRequest("Invalid client");
            }

            var httpClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(_configuration["URI:URL"], cancellationToken);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (discoveryDocument.IsError)
            {
                return BadRequest("Error retrieving discovery document");
            }

            //Revoke AccessToken
            await httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecrets.FirstOrDefault()?.Value,
                Token = accessToken
            }, cancellationToken);

            //Creating a new
            var refreshTokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecrets.FirstOrDefault()?.Value,
                RefreshToken = model.RefreshToken
            }, cancellationToken);

            if (refreshTokenResponse.IsError)
            {
                return BadRequest("Error refreshing token");
            }

            return Ok(new
            {
                AccessToken = refreshTokenResponse.AccessToken,
                RefreshToken = refreshTokenResponse.RefreshToken
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
        }
    }
}
