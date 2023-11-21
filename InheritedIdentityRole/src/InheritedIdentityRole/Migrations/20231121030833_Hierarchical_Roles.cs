using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InheritedIdentityRole.Migrations
{
    /// <inheritdoc />
    public partial class Hierarchical_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "089115bd-f2c6-4288-8bf0-ace89dece997");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d2e82d0-3b1b-47e8-b8c3-4809689f0f9a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bdee975d-6dc7-4664-a077-d9d769693370");

            migrationBuilder.CreateTable(
                name: "RoleHierarchies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ParentRoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ChildRoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleHierarchies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleHierarchies_AspNetRoles_ChildRoleId",
                        column: x => x.ChildRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleHierarchies_AspNetRoles_ParentRoleId",
                        column: x => x.ParentRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3bf13aba-1eb8-475c-96c4-855cc14d1d08", null, "Manager", "MANAGER" },
                    { "93f3ce6f-4ac1-4172-924b-1847e00fbfd7", null, "User", "USER" },
                    { "9e7a820b-affa-4187-9759-8940686b777b", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "RoleHierarchies",
                columns: new[] { "Id", "ChildRoleId", "ParentRoleId" },
                values: new object[,]
                {
                    { "ce802a19-d054-4d63-810d-2268a95eae83", "9e7a820b-affa-4187-9759-8940686b777b", "3bf13aba-1eb8-475c-96c4-855cc14d1d08" },
                    { "f7ffbdb1-ddb4-460f-a8fb-4dc3bcecc503", "3bf13aba-1eb8-475c-96c4-855cc14d1d08", "93f3ce6f-4ac1-4172-924b-1847e00fbfd7" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleHierarchies_ChildRoleId",
                table: "RoleHierarchies",
                column: "ChildRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleHierarchies_ParentRoleId",
                table: "RoleHierarchies",
                column: "ParentRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleHierarchies");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bf13aba-1eb8-475c-96c4-855cc14d1d08");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93f3ce6f-4ac1-4172-924b-1847e00fbfd7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9e7a820b-affa-4187-9759-8940686b777b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "089115bd-f2c6-4288-8bf0-ace89dece997", null, "Administrator", "ADMINISTRATOR" },
                    { "1d2e82d0-3b1b-47e8-b8c3-4809689f0f9a", null, "User", "User" },
                    { "bdee975d-6dc7-4664-a077-d9d769693370", null, "Manager", "MANAGER" }
                });
        }
    }
}
