using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    public Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default!);
}
