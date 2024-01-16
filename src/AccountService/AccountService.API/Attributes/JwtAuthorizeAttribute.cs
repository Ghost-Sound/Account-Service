using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CustomHelper.Authentication.Interfaces;
using CustomHelper.Authentication.NewFolder;

namespace AccountService.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class JwtAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ISignInKeys _signInKeys;
        public string[] _policies { get; }

        public JwtAuthorizeAttribute(
            string[] policies,
            IConfiguration configuration,
            ISignInKeys signInKeys)
        {
            _policies = policies;
            _configuration = configuration;
            _signInKeys = signInKeys;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_policies.Any())
            {
                var authService = context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService)) as IAuthorizationService;

                var isAuthorized = false;

                foreach (var policy in _policies)
                {
                    var authorizationResult = await authService.AuthorizeAsync(context.HttpContext.User, context, policy);
                    if (authorizationResult.Succeeded)
                    {
                        isAuthorized = true;
                        break;
                    }
                }

                if (!isAuthorized)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            var jwtToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = (SecurityKey)await _signInKeys.GetSigninKeys(audience),
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);

                return;
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
