using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    public Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default!);
    public Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default!);
    public Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default!);

}
