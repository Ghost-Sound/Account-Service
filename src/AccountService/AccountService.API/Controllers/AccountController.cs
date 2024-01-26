using AccountService.Infrastructure.DB.Contexts;
using AccountService.Pages;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Duende.IdentityServer.Events;
using IdentityModel.Client;
using RefreshTokenRequest = IdentityModel.Client.RefreshTokenRequest;
using CustomHelper.Authentication.Interfaces;
using MediatR;
using AccountService.Application.Interfaces;
using IAuthenticationService = AccountService.Application.Interfaces.IAuthenticationService;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Attributes;
using CustomHelper.Authentication.Enums;

namespace AccountService.API.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ISignInKeys _signInKeys;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRegistrationService _registrationService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(
            UserDbContext userDbContext,
            ISignInKeys signInKeys,
            IMediator mediator,
            ITokenService tokenService,
            IAuthenticationService authenticationService,
            IRegistrationService registrationService,
            IConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            _signInKeys = signInKeys;
            _tokenService = tokenService;
            _authenticationService = authenticationService;
            _registrationService = registrationService;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserLoginDTO model)
        {
            try
            {
                var result = await _authenticationService.Login(model);

                return Ok(result);
            }
            catch
            {
                throw;
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserRegistryDTO user)
        {
            try
            {
                var urlUser = await _registrationService.Register(user, Url);

                return Created(urlUser.Item1, urlUser.Item2);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorizeAttribute()]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
        {
            try
            {
                var refreshTokenResponse = await _tokenService.RefreshTokenAsync(model, cancellationToken);

                return Ok(new
                {
                    AccessToken = refreshTokenResponse.AccessToken,
                    RefreshToken = refreshTokenResponse.RefreshToken
                });
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
        }
    }
}
