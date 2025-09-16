
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider : IJwtProvider
{
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

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LBaxSXNdx7SSWCqzE8EJFFHAtSpd5KrU"));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "SurveyBasket",
            audience: "SurveyBasketClient",
            claims: calims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials
        );

        return (Token: new JwtSecurityTokenHandler().WriteToken(token), ExpiresIn: 60);
    }
}
