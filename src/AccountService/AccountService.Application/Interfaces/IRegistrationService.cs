using AccountService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<IActionResult> Register(UserRegistryDTO user, IUrlHelper urlHelper);
    }
}