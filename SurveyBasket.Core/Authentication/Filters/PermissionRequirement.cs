using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Core.Authentication.Filters;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}