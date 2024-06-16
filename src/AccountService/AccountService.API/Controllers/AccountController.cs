using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using CustomHelper.Authentication.Interfaces;
using AccountService.Application.Interfaces;
using IAccountAuthenticationService = AccountService.Application.Interfaces.IAccountAuthenticationService;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Attributes;
using Microsoft.AspNetCore.Cors;

namespace AccountService.API.Controllers
{
    [EnableCors("localhost")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountAuthenticationService _authenticationService;
        private readonly IRegistrationService _registrationService;
        private readonly ITokenService _tokenService;

        public AccountController(
            ITokenService tokenService,
            IAccountAuthenticationService authenticationService,
            IRegistrationService registrationService)
        {
            _tokenService = tokenService;
            _authenticationService = authenticationService;
            _registrationService = registrationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            try
            {
                var result = await _authenticationService.Login(model);

                return Ok(new
                {
                    AccessToken = result.Item1,
                    RefreshToken = result.Item2,
                    UserId = result.Item3
                });
            }
            catch
            {
                throw;
            }

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistryDTO user)
        {
            try
            {
                var urlUser = await _registrationService.Register(user);

                return Ok(urlUser);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                var refreshTokenResponse = await _tokenService.RefreshTokenAsync(refreshToken, cancellationToken);

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

        [HttpPost("logOut")]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut(string returnUrl = null)
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
        }
    }
}
