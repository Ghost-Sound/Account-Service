using AccountService.Application.Models.Users;
using Microsoft.AspNetCore.Identity;


namespace AccountService.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<SignInResult> Login(UserLoginDTO model);
    }
}