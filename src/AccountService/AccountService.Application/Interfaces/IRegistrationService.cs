using AccountService.Application.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<(string, UserRegistryDTO)> Register(UserRegistryDTO user, IUrlHelper urlHelper);
    }
}