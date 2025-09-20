namespace SurveyBasket.Api.Contracts.Polls;

public record AuthRequest
    (string Title, string Summary, DateOnly StartsAt, DateOnly EndsAt);
