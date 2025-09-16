using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        bool isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
        {
            return null;
        }

        var (token, expires) = _jwtProvider.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expires);
    }
}
