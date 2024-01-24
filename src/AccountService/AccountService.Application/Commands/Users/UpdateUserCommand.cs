using AccountService.Application.Models.Users;
using AccountService.Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Commands.Users
{
    public record UpdateUserCommand(UserUpdateDTO UserUpdate) : IRequest<User>
    {
    }
}
