using AccountService.Application.Interfaces;
using AccountService.Domain.Entity;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CustomHelper.Exception;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using AccountService.Application.Models.Users;

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

        public async Task<(string, UserRegistryDTO)> Register(UserRegistryDTO user, IUrlHelper urlHelper)
        {
            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                throw new CustomException("User is existed", user);
            }

            try
            {
                await _httpContext.SignInAsync(GetIsuser(user));

                var url = urlHelper.Action(nameof(Register), new { id = 1 }) ?? $"/{1}";

                return (url, user);
            }
            catch
            {
                throw;
            }
        }

        private IdentityServerUser? GetIsuser(UserRegistryDTO user)
        {
            // issue authentication cookie with subject ID and username
            var claims = new List<Claim>();
            return _identityService.CreateIdentityServerUserRegister(user, claims);
        }
    }
}
