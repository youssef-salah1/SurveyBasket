using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Authentication;
using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Services.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpiryDays = 14;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword) return null;

        var (token, expires) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiry
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expires, refreshToken,
            refreshTokenExpiry);
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return null;

        var (newtoken, expires) = _jwtProvider.GenerateToken(user);
        var newrefreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        userRefreshToken.Revoked = DateTime.UtcNow;

        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiry
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newtoken, expires, newrefreshToken,
            refreshTokenExpiry);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.Revoked = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}