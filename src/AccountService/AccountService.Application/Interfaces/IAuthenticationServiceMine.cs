using AccountService.Application.Models.Users;


namespace AccountService.Application.Interfaces
{
    public interface IAuthenticationServiceMine
    {
        Task<(string, string)> Login(UserLoginDTO model);
    }
}