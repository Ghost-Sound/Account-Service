using AccountService.Application.Models.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Queries.User
{
    public record GetUserQuery(Ulid Id) : IRequest<UserGetDTO>
    {
    }
}
