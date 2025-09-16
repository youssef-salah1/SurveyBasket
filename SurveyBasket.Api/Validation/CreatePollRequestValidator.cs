using FluentValidation;
using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Repository.Validation;

public class CreatePollRequestValidator : AbstractValidator<AuthRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.Summary)
            .NotEmpty()
            .MaximumLength(1500);

        RuleFor(p => p.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(p => p.EndsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(p => p.StartsAt);
    }
}
