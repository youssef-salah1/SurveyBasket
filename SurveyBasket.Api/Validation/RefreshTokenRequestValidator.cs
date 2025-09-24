using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Api.Validation;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.Token)
            .NotEmpty();
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}