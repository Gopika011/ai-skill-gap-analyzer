using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_UserId_SkillId",
                table: "EmployeeSkill",
                columns: new[] { "UserId", "SkillId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSkill_Users_UserId",
                table: "EmployeeSkill",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkill_Users_UserId",
                table: "EmployeeSkill");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSkill_UserId_SkillId",
                table: "EmployeeSkill");
        }
    }
}
