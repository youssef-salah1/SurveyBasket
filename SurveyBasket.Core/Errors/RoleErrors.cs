using Microsoft.AspNetCore.Http;
using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = new("Role.NotFound", "Role not found",
        StatusCodes.Status404NotFound);

    public static readonly Error InvalidPermissions =
        new("Role.InvalidPermissions", "Invalid permissions", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicateRole = new("Role.DuplicateRole",
        "A role with the same name already exists.", StatusCodes.Status409Conflict);
}