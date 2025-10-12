using SurveyBasket.Core.Abstractions.Consts;
using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Api.Validation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.UserName)
            .NotEmpty()
            .Length(3, 20)
            .Matches(RegexPatterns.UserName)
            .WithMessage("Username can only contain letters, digits, and these characters: - . _ @ +");
        
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters and include at least one lowercase letter, one uppercase letter, one digit, and one special character.");

        RuleFor(r => r.FirstName)
            .NotEmpty()
            .Length(3, 100);
        RuleFor(r => r.LastName)
            .NotEmpty()
            .Length(3, 100);
    }
}