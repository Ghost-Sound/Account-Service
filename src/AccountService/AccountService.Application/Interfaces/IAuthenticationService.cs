using AccountService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<IActionResult> Login(UserLoginDTO model);
    }
}