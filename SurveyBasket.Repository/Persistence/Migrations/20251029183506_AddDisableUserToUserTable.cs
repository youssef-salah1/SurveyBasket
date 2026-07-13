using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Repository.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDisableUserToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisable",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019a1c20-390c-704d-92e1-7dcf93597854",
                column: "IsDisable",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisable",
                table: "AspNetUsers");
        }
    }
}
