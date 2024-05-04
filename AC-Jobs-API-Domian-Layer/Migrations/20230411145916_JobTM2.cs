using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class JobTM2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductioManagerId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "Jobs",
                newName: "SubContractorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubContractorId",
                table: "Jobs",
                newName: "TagId");

            migrationBuilder.AddColumn<long>(
                name: "ProductioManagerId",
                table: "Jobs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
