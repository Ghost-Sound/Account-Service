using AccountService.Application.Models.Departments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Queries.Departments
{
    public record GetDepartmentQuery(Ulid Ulid) : IRequest<GetDepartmentDTO>
    {
    }
}
