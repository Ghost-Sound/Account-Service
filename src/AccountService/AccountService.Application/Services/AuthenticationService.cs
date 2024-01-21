using AccountService.Application.Interfaces;
using AccountService.Application.Models;
using AccountService.Domain.Entity;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


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

        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new BadRequestObjectResult(model);
            }

            if (user.EmailConfirmed == false)
            {
                return new BadRequestObjectResult(new { message = "User not confirmed email", errors = user });
            }

            var sub = _identityService.CreateUniqueId();

            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, sub, user.UserName, clientId: context?.Client.ClientId));

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Succeeded);
            }

            // only set explicit expiration here if user chooses "remember me". 
            // otherwise we rely upon expiration configured in cookie middleware.
            AuthenticationProperties? props = _identityService.CreateAuthenticationProperties(model.RememberLogin);

            // issue authentication cookie with subject ID and username
            var claims = new List<Claim>();
            var isuser = _identityService.CreateIdentityServerUser(user, claims);

            await _httpContext.SignInAsync(isuser, props);

            return new OkObjectResult(result);
        }
    }
}
