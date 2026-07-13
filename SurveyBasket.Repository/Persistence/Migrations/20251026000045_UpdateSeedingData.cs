using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Repository.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a1c20-390e-7fd8-9b20-cde0cc78e33e",
                column: "IsDefualt",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019a1c20-390e-7fd8-9b20-cde0cc78e33e",
                column: "IsDefualt",
                value: false);
        }
    }
}
