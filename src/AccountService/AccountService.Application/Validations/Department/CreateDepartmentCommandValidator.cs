using AccountService.Application.Models.Departments;
using FluentValidation;

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
