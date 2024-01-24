using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Commands.Departments
{
    public record DeleteDepartmentCommand(Ulid Ulid): IRequest<bool>
    {
    }
}
