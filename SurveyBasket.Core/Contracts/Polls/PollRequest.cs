namespace SurveyBasket.Api.Contracts.Polls;

public record AuthRequest
    (string Title, string Summary, bool IsPublished, DateOnly StartsAt, DateOnly EndsAt);
