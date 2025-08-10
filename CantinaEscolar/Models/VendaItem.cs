using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantinaEscolar.Models
{
    public class VendaItem
    {
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; }
        public Venda? Venda { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        [Range(1, 100000)]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; } // congela o preço no momento da venda

        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
