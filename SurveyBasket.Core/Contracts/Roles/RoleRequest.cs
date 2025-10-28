namespace SurveyBasket.Core.Contracts.Roles;

public record RoleRequest(string Name , IList<string> permissions);