using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountService.Application.Models.UserDTO;

namespace AccountService.Application.Commands.Users
{
    public record UpdateUserCommand(UserUpdateDTO UserUpdate) : IRequest<UserUpdateDTO>
    {
    }
}
