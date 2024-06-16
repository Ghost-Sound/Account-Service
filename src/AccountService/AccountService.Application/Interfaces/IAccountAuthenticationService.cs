using AccountService.Application.Models.Users;


namespace AccountService.Application.Interfaces
{
    public interface IAccountAuthenticationService
    {
        Task<(string, string, string)> Login(UserLoginDTO model);
    }
}