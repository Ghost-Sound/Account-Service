using AccountService.Application.Models.Users;
using MediatR;

namespace AccountService.Application.Queries.User
{
    public record GetUsersQuery(UsersGetDTO Users) : IRequest<IEnumerable<UserGetDTO>>
    {
    }
}
