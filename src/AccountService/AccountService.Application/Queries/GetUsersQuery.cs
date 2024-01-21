using AccountService.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountService.Application.Models.UserDTO;

namespace AccountService.Application.Queries
{
    public record GetUsersQuery(UsersGetDTO Users) : IRequest<IEnumerable<UserGetDTO>>
    {
    }
}
