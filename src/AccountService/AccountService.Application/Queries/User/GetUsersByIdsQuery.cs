using AccountService.Application.Models.Users;
using MediatR;

namespace AccountService.Application.Queries.User
{
    public record GetUsersByIdsQuery(List<string> Ids) : IRequest<List<UserGetDTO>>
    {
    }
}
