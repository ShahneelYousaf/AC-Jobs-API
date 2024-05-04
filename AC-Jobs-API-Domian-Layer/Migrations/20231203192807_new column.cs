using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class newcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "JobAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "JobAttachments");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Attachments");
        }
    }
}
