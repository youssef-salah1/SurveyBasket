using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Entities;
using SurveyBasket.Core.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, int ExpiresIn) GenerateToken(ApplicationUser applicationUser)
    {
        Claim[] calims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName , applicationUser.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName , applicationUser.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: calims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: signingCredentials
        );

        return (Token: new JwtSecurityTokenHandler().WriteToken(token), ExpiresIn: _options.ExpiryMinutes * 60);
    }
}
