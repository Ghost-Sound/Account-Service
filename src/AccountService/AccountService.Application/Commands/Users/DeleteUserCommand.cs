using AccountService.Application.Models.Users;
using AccountService.Domain.Entity;
using MediatR;

namespace AccountService.Application.Commands.Users
{
    public record DeleteUserCommand(Ulid Id) : IRequest<User>
    {
    }
}
