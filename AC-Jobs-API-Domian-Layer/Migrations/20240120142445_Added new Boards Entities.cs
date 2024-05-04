using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class AddednewBoardsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boardEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoardAccessUserEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BoardEntityId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardAccessUserEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardAccessUserEntity_boardEntities_BoardEntityId",
                        column: x => x.BoardEntityId,
                        principalTable: "boardEntities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "boardStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SortBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SortingOrder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Total = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boardStatuses_boardEntities_BoardId",
                        column: x => x.BoardId,
                        principalTable: "boardEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "boardWorkFlowStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkFlowId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boardWorkFlowStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boardWorkFlowStatuses_boardStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "boardStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardAccessUserEntity_BoardEntityId",
                table: "BoardAccessUserEntity",
                column: "BoardEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_boardStatuses_BoardId",
                table: "boardStatuses",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_boardWorkFlowStatuses_StatusId",
                table: "boardWorkFlowStatuses",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardAccessUserEntity");

            migrationBuilder.DropTable(
                name: "boardWorkFlowStatuses");

            migrationBuilder.DropTable(
                name: "boardStatuses");

            migrationBuilder.DropTable(
                name: "boardEntities");
        }
    }
}
