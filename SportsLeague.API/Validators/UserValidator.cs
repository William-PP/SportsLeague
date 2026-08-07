using FluentValidation;
using SportsLeague.Domain.Entities;

namespace SportsLeague.API.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(80).WithMessage("First name must not exceed 80 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(80).WithMessage("Last name must not exceed 80 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters");
    }
}