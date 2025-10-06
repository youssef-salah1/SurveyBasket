namespace SurveyBasket.Core.Contracts.Results;

public record VotesPerAnswerResponse(
    string Answer,
    int Count
);