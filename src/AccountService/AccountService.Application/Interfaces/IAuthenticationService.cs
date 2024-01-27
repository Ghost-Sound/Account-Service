using AccountService.Application.Models.Users;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;


namespace AccountService.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> Login(UserLoginDTO model);
    }
}