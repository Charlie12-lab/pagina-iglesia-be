using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IglesiaNet.Infrastructure.Persistence.SQL.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchEventBlogNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modality",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Denomination",
                table: "Churches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastorEmail",
                table: "Churches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PastorName",
                table: "Churches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Churches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Modality",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Denomination",
                table: "Churches");

            migrationBuilder.DropColumn(
                name: "PastorEmail",
                table: "Churches");

            migrationBuilder.DropColumn(
                name: "PastorName",
                table: "Churches");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Churches");
        }
    }
}
