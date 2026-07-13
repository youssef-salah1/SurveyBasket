namespace SurveyBasket.Core.Contracts.Results;

public record PollVotesResponse(
    string Title,
    DateOnly StartsAt,
    DateOnly EndsAt,
    IEnumerable<VoteResponse> Votes
);