using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Abstractions.Consts;

namespace SurveyBasket.Repository.Persistence.EntitiesConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData([
            new IdentityUserRole<string>
        {
                RoleId = DefaultRoles.AdminRoleId,
                UserId = DefaultUsers.AdminId
        }]);
    }
}