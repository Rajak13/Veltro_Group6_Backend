using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veltro.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reviews");
        }
    }
}
