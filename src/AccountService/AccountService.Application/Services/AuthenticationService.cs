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

namespace AccountService.Application.Services
{
    public class AuthenticationService : Interfaces.IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IIdentityService _identityService;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpContext _httpContext;

        public AuthenticationService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IIdentityService identityService,
            IEventService events,
            IIdentityServerInteractionService interaction,
            IHttpClientFactory httpClientFactory,
            HttpContext httpContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityService = identityService;
            _events = events;
            _interaction = interaction;
            _httpClientFactory = httpClientFactory;
            _httpContext = httpContext;
        }

        public async Task<SignInResult> Login(UserLoginDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    throw new CustomException(message: typeof(UserLoginDTO).FullName, user);
                }

                if (user.EmailConfirmed == false)
                {
                    throw new CustomException(message: "User not confirmed email", user);
                }
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    throw new CustomException("Not succeeded login");
                }

                await _httpContext.SignInAsync(GetIsuser(user), GetProperties(model.RememberLogin));

                return result;
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
            var claims = new List<Claim>();
            return _identityService.CreateIdentityServerUser(user, claims);
        }
    }
}
