using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CantinaEscolar.Migrations
{
    /// <inheritdoc />
    public partial class addAtualizacaoTabelaResponsavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataPgto",
                table: "Responsaveis",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Responsaveis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fone",
                table: "Responsaveis",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPgto",
                table: "Responsaveis");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Responsaveis");

            migrationBuilder.DropColumn(
                name: "Fone",
                table: "Responsaveis");
        }
    }
}
