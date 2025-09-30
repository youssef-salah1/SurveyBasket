using SurveyBasket.Core.Contracts.Question;

namespace SurveyBasket.Api.Validation;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(q => q.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(q => q.Answers)
            .NotNull();

        RuleFor(q => q.Answers)
            .Must(q => q.Count >= 2)
            .WithMessage("At least two answers are required.")
            .When(q => q.Answers is not null);

        RuleFor(q => q.Answers)
            .Must(q => q.Distinct().Count() == q.Count)
            .WithMessage("Answers must be unique.")
            .When(q => q.Answers is not null);
    }
}