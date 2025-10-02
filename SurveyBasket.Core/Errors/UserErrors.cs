using Microsoft.AspNetCore.Http;
using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new("User.InvalidCredentials", "Invalid email/password",
        StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InValidRefreshToken = new("User.InvalidRefreshToken", "Invalid refresh token.",
        StatusCodes.Status401Unauthorized);
}