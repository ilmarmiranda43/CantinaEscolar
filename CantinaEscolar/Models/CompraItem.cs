using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantinaEscolar.Models
{
    public class CompraItem
    {
        public int Id { get; set; }

        [Required]
        public int CompraId { get; set; }
        public Compra Compra { get; set; } = default!;

        [Required]
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = default!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser no mínimo 1.")]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999)]
        public decimal PrecoUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
