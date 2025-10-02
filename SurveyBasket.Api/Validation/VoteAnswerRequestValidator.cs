using SurveyBasket.Core.Contracts.Vote;

namespace SurveyBasket.Api.Validation;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0).WithMessage("QuestionId must be greater than 0.");
        RuleFor(x => x.AnswerId)
            .GreaterThan(0).WithMessage("AnswerId must be greater than 0.");
    }
}