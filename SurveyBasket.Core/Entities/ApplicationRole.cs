using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Core.Entities;

public class ApplicationRole : IdentityRole
{
    public bool IsDefualt { get; set; }
    public bool IsDeleted { get; set; }
}
