using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Api.Validation;

public class AuthRequestValidator : AbstractValidator<LoginRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}