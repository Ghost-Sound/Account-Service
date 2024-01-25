using AccountService.Application.Models.Departments;
using AccountService.Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Commands.Departments
{
    public record UpdateDepartmentCommand(UpdateDepartmentDTO UpdateDepartment): IRequest<Department>
    {
    }
}
