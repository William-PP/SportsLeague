using FluentValidation;
using SportsLeague.API.DTOs.Request;

namespace SportsLeague.API.Validators;

public class LoginValidator : AbstractValidator<AuthRequestDTO>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}