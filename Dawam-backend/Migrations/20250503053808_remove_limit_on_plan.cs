using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dawam_backend.Migrations
{
    /// <inheritdoc />
    public partial class remove_limit_on_plan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyApplicationLimit",
                table: "SubscriptionPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyApplicationLimit",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
