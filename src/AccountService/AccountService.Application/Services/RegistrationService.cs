using AccountService.Application.Interfaces;
using AccountService.Application.Models;
using AccountService.Domain.Entity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly HttpContext _httpContext;

        public RegistrationService(
            UserManager<User> userManager,
            IIdentityService identityService,
            IIdentityServerInteractionService interaction,
            HttpContext httpContext)
        {
            _userManager = userManager;
            _identityService = identityService;
            _interaction = interaction;
            _httpContext = httpContext;
        }

        public async Task<IActionResult> Register(UserRegistryDTO user, IUrlHelper urlHelper)
        {
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

            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                return new ConflictObjectResult(user);
            }

            var claims = new List<Claim>();

            var isuser = _identityService.CreateIdentityServerUserRegister(user, claims);

            await _httpContext.SignInAsync(isuser);

            var url = urlHelper.Action(nameof(Register), new { id = 1 }) ?? $"/{1}";

            return new CreatedAtRouteResult(url, user);
        }
    }
}
