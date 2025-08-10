using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CantinaEscolar.Migrations
{
    /// <inheritdoc />
    public partial class AlterarParaDiaPgto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPgto",
                table: "Responsaveis");

            migrationBuilder.AddColumn<int>(
                name: "DiaPgto",
                table: "Responsaveis",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaPgto",
                table: "Responsaveis");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPgto",
                table: "Responsaveis",
                type: "datetime2",
                nullable: true);
        }
    }
}
