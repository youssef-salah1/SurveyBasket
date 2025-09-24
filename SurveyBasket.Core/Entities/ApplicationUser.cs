using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Core.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<RefreshTokens> RefreshTokens { get; set; } = [];
}