using SurveyBasket.Core.Entities;

namespace SurveyBasket.Core.Authentication;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateToken(ApplicationUser applicationUser);
    public string? ValidateToken(string token);
}