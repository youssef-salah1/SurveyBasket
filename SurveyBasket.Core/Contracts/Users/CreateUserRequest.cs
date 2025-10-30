namespace SurveyBasket.Core.Contracts.Users;

public record CreateUserRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password,
    IList<string> Roles
    );
