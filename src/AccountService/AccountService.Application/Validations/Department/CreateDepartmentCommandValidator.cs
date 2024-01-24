using AccountService.Application.Commands.Users;
using AccountService.Application.Models.Departments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Validations.Department
{
    public class CreateDepartmentCommandValidator: AbstractValidator<CreateDepartmentDTO>
    {
        public CreateDepartmentCommandValidator() 
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
