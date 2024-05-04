using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class addednewnamescolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "statusName",
                table: "boardWorkFlowStatuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "workFlowName",
                table: "boardWorkFlowStatuses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statusName",
                table: "boardWorkFlowStatuses");

            migrationBuilder.DropColumn(
                name: "workFlowName",
                table: "boardWorkFlowStatuses");
        }
    }
}
