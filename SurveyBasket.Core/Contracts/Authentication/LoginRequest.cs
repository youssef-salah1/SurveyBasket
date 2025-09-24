namespace SurveyBasket.Core.Contracts.Authentication;

public record LoginRequest(string Email, string Password);

//public class LoginRequest
//{
//    [EmailAddress]
//    public string Email { get; set; } = string.Empty;
//    public string Password { get; set; } = string.Empty;
//}