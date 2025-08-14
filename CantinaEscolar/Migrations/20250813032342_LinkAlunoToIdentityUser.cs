using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CantinaEscolar.Migrations
{
    /// <inheritdoc />
    public partial class LinkAlunoToIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Alunos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_ApplicationUserId",
                table: "Alunos",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_AspNetUsers_ApplicationUserId",
                table: "Alunos",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_AspNetUsers_ApplicationUserId",
                table: "Alunos");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_ApplicationUserId",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Alunos");
        }
    }
}
