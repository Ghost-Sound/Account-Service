using AccountService.Application.Interfaces;
using AccountService.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CustomHelper.Exception;
using Duende.IdentityServer;
using AccountService.Application.Models.Users;
using AutoMapper;
using CustomHelper.Authentication.Enums;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(
            UserManager<User> userManager,
            IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<RegistrationService> logger)
        {
            _userManager = userManager;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserRegistryDTO> Register(UserRegistryDTO model)
        {
            var dockerUrl = Environment.GetEnvironmentVariable("DOCKER_URL");

            _logger.LogDebug("Docker URL: {URL}", dockerUrl);
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                throw new CustomException("User is existed", model);
            }

            try
            {
                var user = _mapper.Map<User>(model);

                user.Id = Ulid.NewUlid();
                user.LastSuccessfullEmailVerification = DateTime.UtcNow;
                user.LastSuccessfullLogin = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(user, model.Password);

                if(!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new Exception($"Failed to create user. Errors: {string.Join(", ", errors)}");
                }

                await AddClaimsAndRolesToUser(user);

                await _httpContextAccessor.HttpContext.SignInAsync(GetIsuser(model));

                return model;
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
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
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
