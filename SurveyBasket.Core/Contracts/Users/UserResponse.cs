namespace SurveyBasket.Core.Contracts.Users;

public record UserResponse
(string Id,
 string UserName,
 string Email,
 string FirstName,
 string LastName,
 bool IsDisable,
 IEnumerable<string> Roles);