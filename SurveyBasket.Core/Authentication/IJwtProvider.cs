using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateToken(ApplicationUser applicationUser);
}
