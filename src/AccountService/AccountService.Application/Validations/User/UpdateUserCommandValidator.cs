using AccountService.Application.Commands.Users;
using FluentValidation;

namespace AccountService.Application.Validations.User
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.UserUpdate.Id).NotEmpty();
            RuleFor(x => x.UserUpdate.FirstName).MaximumLength(100);
            RuleFor(x => x.UserUpdate.LastName).MaximumLength(100);
            RuleFor(x => x.UserUpdate.MiddleName).MaximumLength(100);
        }
    }
}
