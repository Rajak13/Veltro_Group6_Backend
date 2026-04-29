using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veltro.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "Appointments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Appointments");
        }
    }
}
