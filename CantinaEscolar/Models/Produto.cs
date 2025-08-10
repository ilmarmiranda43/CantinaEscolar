using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantinaEscolar.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 999999)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        [Range(0, 100000)]
        public int Quantidade { get; set; }

        [StringLength(80)]
        public string? Categoria { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
