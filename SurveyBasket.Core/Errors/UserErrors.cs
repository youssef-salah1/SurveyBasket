using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new("User.NotFound", "User not found.");
    public static readonly Error InValidRefreshToken = new("User.InvalidRefreshToken", "Invalid refresh token.");
}