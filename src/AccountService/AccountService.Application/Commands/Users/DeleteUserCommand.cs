using AccountService.Application.Models.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountService.Application.Models.Users.UserDTO;

namespace AccountService.Application.Commands.Users
{
    public record DeleteUserCommand(UserDeleteDTO UserDelete) : IRequest<bool>
    {
    }
}
