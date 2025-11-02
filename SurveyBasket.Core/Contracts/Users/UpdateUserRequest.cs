namespace SurveyBasket.Core.Contracts.Users;

public record UpdateUserRequest
(string UserName,
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles);