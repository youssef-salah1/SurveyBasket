namespace SurveyBasket.Core.Contracts.Results;

public record VotesPerDayResponse(
    DateOnly Date,
    int NumberOfVotes
);