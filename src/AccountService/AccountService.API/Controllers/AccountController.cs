using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using CustomHelper.Authentication.Interfaces;
using AccountService.Application.Interfaces;
using IAuthenticationServiceMine = AccountService.Application.Interfaces.IAuthenticationServiceMine;
using AccountService.Application.Models.Users;
using CustomHelper.Authentication.Attributes;

namespace AccountService.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ISignInKeys _signInKeys;
        private readonly IAuthenticationServiceMine _authenticationService;
        private readonly IRegistrationService _registrationService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(
            ISignInKeys signInKeys,
            ITokenService tokenService,
            IAuthenticationServiceMine authenticationService,
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

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            try
            {
                var result = await _authenticationService.Login(model);

                return Ok(new
                {
                    AccessToken = result.Item1,
                    RefreshToken = result.Item2
                });
            }
            catch
            {
                throw;
            }

        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserRegistryDTO user)
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
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");
        }
    }
}
