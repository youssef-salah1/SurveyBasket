using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Repository.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefualt",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefualt", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019a1c20-390e-7fd8-9b20-cddc38906b5b", "019a1c20-390e-7fd8-9b20-cdddf127ba16", false, false, "Admin", "ADMIN" },
                    { "019a1c20-390e-7fd8-9b20-cde0cc78e33e", "019a1c20-390e-7fd8-9b20-cddf89d2a037", false, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019a1c20-390c-704d-92e1-7dcf93597854", 0, "019a1c20-390e-7fd8-9b20-cdde028f1737", "Admin@surveybasket.org", true, "System", "Admin", false, null, "ADMIN@SURVEYBASKET.ORG", "ADMIN", "AQAAAAIAAYagAAAAEFA4YWPPxGVMatlhTmfO3c1bCnlplmdko15WPEqSPxfeKLPxLFzDTa7n+y73KPyLgQ==", null, false, "019a1c20390e7fd89b20cddb68eed9f5", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "polls:read", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 2, "Permissions", "polls:add", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 3, "Permissions", "polls:update", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 4, "Permissions", "polls:delete", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 5, "Permissions", "questions:read", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 6, "Permissions", "questions:add", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 7, "Permissions", "questions:update", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 8, "Permissions", "users:read", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 9, "Permissions", "users:add", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 10, "Permissions", "users:update", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 11, "Permissions", "roles:read", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 12, "Permissions", "roles:add", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 13, "Permissions", "roles:update", "019a1c20-390e-7fd8-9b20-cddc38906b5b" },
                    { 14, "Permissions", "results:read", "019a1c20-390e-7fd8-9b20-cddc38906b5b" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019a1c20-390e-7fd8-9b20-cddc38906b5b", "019a1c20-390c-704d-92e1-7dcf93597854" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a1c20-390e-7fd8-9b20-cde0cc78e33e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019a1c20-390e-7fd8-9b20-cddc38906b5b", "019a1c20-390c-704d-92e1-7dcf93597854" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a1c20-390e-7fd8-9b20-cddc38906b5b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019a1c20-390c-704d-92e1-7dcf93597854");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefualt",
                table: "AspNetRoles",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
