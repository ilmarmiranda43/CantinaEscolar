using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CantinaEscolar.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoToCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Compras",
                newName: "ValorTotal");

            migrationBuilder.CreateTable(
                name: "CompraItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompraItens_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraItens_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraItens_CompraId",
                table: "CompraItens",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraItens_ProdutoId",
                table: "CompraItens",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraItens");

            migrationBuilder.RenameColumn(
                name: "ValorTotal",
                table: "Compras",
                newName: "Valor");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Compras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
