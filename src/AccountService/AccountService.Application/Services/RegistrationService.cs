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
using AutoMapper;
using CustomHelper.Authentication.Enums;

namespace AccountService.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public RegistrationService(
            UserManager<User> userManager,
            IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userManager = userManager;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<(string, UserRegistryDTO)> Register(UserRegistryDTO model, IUrlHelper urlHelper)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                throw new CustomException("User is existed", model);
            }

            try
            {
                var user = _mapper.Map<User>(model);

                user.Id = Ulid.NewUlid();

                var result = await _userManager.CreateAsync(user, model.Password);

                if(!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new Exception($"Failed to create user. Errors: {string.Join(", ", errors)}");
                }

                await AddClaimsAndRolesToUser(user);

                await _httpContextAccessor.HttpContext.SignInAsync(GetIsuser(model));

                var url = urlHelper.Action(nameof(Register), new { id = user.Id }) ?? $"/{user.Id}";

                return (url, model);
            }
            catch
            {
                throw;
            }
        }

        private async Task AddClaimsAndRolesToUser(User user)
        {
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, UserRoles.Student.ToString()));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            await _userManager.AddToRoleAsync(user, UserRoles.Student.ToString());
        }

        private IdentityServerUser? GetIsuser(UserRegistryDTO user)
        {
            // issue authentication cookie with subject ID and username
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, UserRoles.Student.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            return _identityService.CreateIdentityServerUserRegister(user, claims);
        }
    }
}
