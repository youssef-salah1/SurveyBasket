using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Abstractions.Consts;

namespace SurveyBasket.Repository.Persistence.EntitiesConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(
            p => p.RefreshTokens,
            a =>
            {
                a.WithOwner().HasForeignKey("UserId");
                //a.Property<int>("Id");
                //a.HasKey("UserId", "Id");
            }
        );
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData([
            new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            UserName = DefaultUsers.AdminUsername,
            NormalizedUserName = DefaultUsers.AdminUsername.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Admin",
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            PasswordHash = "AQAAAAIAAYagAAAAEFA4YWPPxGVMatlhTmfO3c1bCnlplmdko15WPEqSPxfeKLPxLFzDTa7n+y73KPyLgQ=="
        }]);
    }
}