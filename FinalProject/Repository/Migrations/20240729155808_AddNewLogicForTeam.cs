using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class AddNewLogicForTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Socials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Socials_TeamId",
                table: "Socials",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Socials_Teams_TeamId",
                table: "Socials",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socials_Teams_TeamId",
                table: "Socials");

            migrationBuilder.DropIndex(
                name: "IX_Socials_TeamId",
                table: "Socials");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Socials");
        }
    }
}
