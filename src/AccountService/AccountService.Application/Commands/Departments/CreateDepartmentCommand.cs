using AccountService.Application.Models.Departments;
using MediatR;

namespace AccountService.Application.Commands.Departments
{
    public record CreateDepartmentCommand(CreateDepartmentDTO CreateDepartment) : IRequest<CreateDepartmentDTO>
    {
    }
}
