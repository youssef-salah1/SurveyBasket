using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Entities;

namespace SurveyBasket.Api.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<RefreshTokens> RefreshTokens { get; set; } = [];
}
