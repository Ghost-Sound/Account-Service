using AccountService.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Queries
{
    public record GetUserQuery(Ulid Id) : IRequest<UserGetDTO>
    {
    }
}
