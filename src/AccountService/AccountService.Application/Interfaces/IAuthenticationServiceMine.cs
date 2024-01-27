using AccountService.Application.Models.Users;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;


namespace AccountService.Application.Interfaces
{
    public interface IAuthenticationServiceMine
    {
        Task<string> Login(UserLoginDTO model);
    }
}