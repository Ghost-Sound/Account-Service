using AccountService.Application.Models.Users;
using MediatR;

namespace AccountService.Application.Commands.Users
{
    public record CreateUserCommand(UserRegistryDTO User) : IRequest<UserRegistryDTO>
    {
    }
}
