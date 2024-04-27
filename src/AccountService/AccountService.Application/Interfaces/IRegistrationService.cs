using AccountService.Application.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<UserRegistryDTO> Register(UserRegistryDTO user);
    }
}