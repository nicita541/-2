using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncDomainFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_projects_WorkspaceId",
                table: "projects");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "workspaces",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "task_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "task_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "projects",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "projects",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "projects",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "projects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "project_notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ContentMarkdown = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_project_notes_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_items_ProjectId_Status_Priority",
                table: "task_items",
                columns: new[] { "ProjectId", "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_projects_WorkspaceId_Status_IsArchived",
                table: "projects",
                columns: new[] { "WorkspaceId", "Status", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_project_notes_ProjectId",
                table: "project_notes",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_notes");

            migrationBuilder.DropIndex(
                name: "IX_task_items_ProjectId_Status_Priority",
                table: "task_items");

            migrationBuilder.DropIndex(
                name: "IX_projects_WorkspaceId_Status_IsArchived",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "workspaces");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "task_items");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "task_items");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "projects");

            migrationBuilder.CreateIndex(
                name: "IX_projects_WorkspaceId",
                table: "projects",
                column: "WorkspaceId");
        }
    }
}
