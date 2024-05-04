using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC_Jobs_API_Domian_Layer.Migrations
{
    /// <inheritdoc />
    public partial class Addedtrackingcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationActions_Automations_AutomationEntityId",
                table: "AutomationActions");

            migrationBuilder.DropForeignKey(
                name: "FK_AutomationConditions_Automations_AutomationEntityId",
                table: "AutomationConditions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecutionTime",
                table: "Automations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSuccessfulExecutionTime",
                table: "Automations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextExecutionTime",
                table: "Automations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationActions_Automations_AutomationEntityId",
                table: "AutomationActions",
                column: "AutomationEntityId",
                principalTable: "Automations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationConditions_Automations_AutomationEntityId",
                table: "AutomationConditions",
                column: "AutomationEntityId",
                principalTable: "Automations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomationActions_Automations_AutomationEntityId",
                table: "AutomationActions");

            migrationBuilder.DropForeignKey(
                name: "FK_AutomationConditions_Automations_AutomationEntityId",
                table: "AutomationConditions");

            migrationBuilder.DropColumn(
                name: "LastExecutionTime",
                table: "Automations");

            migrationBuilder.DropColumn(
                name: "LastSuccessfulExecutionTime",
                table: "Automations");

            migrationBuilder.DropColumn(
                name: "NextExecutionTime",
                table: "Automations");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationActions_Automations_AutomationEntityId",
                table: "AutomationActions",
                column: "AutomationEntityId",
                principalTable: "Automations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomationConditions_Automations_AutomationEntityId",
                table: "AutomationConditions",
                column: "AutomationEntityId",
                principalTable: "Automations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
