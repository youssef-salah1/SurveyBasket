using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Authentication;
using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Services.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpiryDays = 14;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var (token, expires) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiry
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expires,
            refreshToken,
            refreshTokenExpiry);

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InValidRefreshToken);

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

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newtoken, expires,
            newrefreshToken,
            refreshTokenExpiry);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.UserNotFound);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InValidRefreshToken);

        userRefreshToken.Revoked = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}