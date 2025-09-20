namespace SurveyBasket.Core.Contracts.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);