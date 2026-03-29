using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IglesiaNet.Infrastructure.Persistence.SQL.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherPriceGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Events",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "EventRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EventRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Church",
                table: "EventRegistrations",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "EventRegistrations",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherPath",
                table: "EventRegistrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Church",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "VoucherPath",
                table: "EventRegistrations");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "EventRegistrations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EventRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
