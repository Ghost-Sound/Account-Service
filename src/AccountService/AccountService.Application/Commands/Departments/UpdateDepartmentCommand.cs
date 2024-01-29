using AccountService.Application.Models.Departments;
using AccountService.Domain.Entity;
using MediatR;

namespace AccountService.Application.Commands.Departments
{
    public record UpdateDepartmentCommand(UpdateDepartmentDTO UpdateDepartment): IRequest<Department>
    {
    }
}
