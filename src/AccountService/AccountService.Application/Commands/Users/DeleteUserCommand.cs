using AccountService.Application.Models.Users;
using MediatR;

namespace AccountService.Application.Commands.Users
{
    public record DeleteUserCommand(Ulid Id) : IRequest<bool>
    {
    }
}
