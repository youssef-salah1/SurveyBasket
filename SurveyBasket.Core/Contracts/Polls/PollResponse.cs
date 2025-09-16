namespace SurveyBasket.Api.Contracts.Polls;

public record AuthResponse
    (int Id, string Title, string Summary, bool IsPublished, DateOnly StartsAt, DateOnly EndsAt);