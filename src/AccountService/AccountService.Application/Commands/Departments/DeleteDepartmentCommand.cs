using AccountService.Domain.Entity;
using MediatR;

namespace AccountService.Application.Commands.Departments
{
    public record DeleteDepartmentCommand(Ulid Ulid): IRequest<Department>
    {
    }
}
