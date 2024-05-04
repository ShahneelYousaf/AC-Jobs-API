using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class AddednewBoardsEntities1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardAccessUserEntity_boardEntities_BoardEntityId",
                table: "BoardAccessUserEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardAccessUserEntity",
                table: "BoardAccessUserEntity");

            migrationBuilder.RenameTable(
                name: "BoardAccessUserEntity",
                newName: "boardAccessUsers");

            migrationBuilder.RenameIndex(
                name: "IX_BoardAccessUserEntity_BoardEntityId",
                table: "boardAccessUsers",
                newName: "IX_boardAccessUsers_BoardEntityId");

            migrationBuilder.AlterColumn<long>(
                name: "WorkFlowId",
                table: "boardWorkFlowStatuses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "boardAccessUsers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "AccessUserId",
                table: "boardAccessUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_boardAccessUsers",
                table: "boardAccessUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_boardAccessUsers_boardEntities_BoardEntityId",
                table: "boardAccessUsers",
                column: "BoardEntityId",
                principalTable: "boardEntities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boardAccessUsers_boardEntities_BoardEntityId",
                table: "boardAccessUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_boardAccessUsers",
                table: "boardAccessUsers");

            migrationBuilder.DropColumn(
                name: "AccessUserId",
                table: "boardAccessUsers");

            migrationBuilder.RenameTable(
                name: "boardAccessUsers",
                newName: "BoardAccessUserEntity");

            migrationBuilder.RenameIndex(
                name: "IX_boardAccessUsers_BoardEntityId",
                table: "BoardAccessUserEntity",
                newName: "IX_BoardAccessUserEntity_BoardEntityId");

            migrationBuilder.AlterColumn<int>(
                name: "WorkFlowId",
                table: "boardWorkFlowStatuses",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "BoardAccessUserEntity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardAccessUserEntity",
                table: "BoardAccessUserEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardAccessUserEntity_boardEntities_BoardEntityId",
                table: "BoardAccessUserEntity",
                column: "BoardEntityId",
                principalTable: "boardEntities",
                principalColumn: "Id");
        }
    }
}
