using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Core.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}