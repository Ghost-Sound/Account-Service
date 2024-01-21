using AccountService.Infrastructure.DB.Contexts;
using AccountService.Pages;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountService.Application.Models;
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

namespace AccountService.API.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        private readonly ISignInKeys _signInKeys;
        private readonly IMediator _mediator;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRegistrationService _registrationService;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserDbContext userDbContext,
            ISignInKeys signInKeys,
            IMediator mediator,
            ITokenService tokenService)
        {
            _dbContext = userDbContext;
            _signInKeys = signInKeys;
            _mediator = mediator;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserLoginDTO model)
        {
            return await _authenticationService.Login(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserRegistryDTO user)
        {
            return await _registrationService.Register(user, Url);
        }

        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
        {
            var refreshTokenResponse = await _tokenService.RefreshTokenAsync(model, cancellationToken);

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
