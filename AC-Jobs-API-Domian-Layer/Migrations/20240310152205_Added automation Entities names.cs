using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class AddedautomationEntitiesnames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RightOperand",
                table: "AutomationConditions",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "AutomationConditions",
                newName: "Comparison");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AutomationConditions",
                newName: "RightOperand");

            migrationBuilder.RenameColumn(
                name: "Comparison",
                table: "AutomationConditions",
                newName: "Condition");
        }
    }
}
