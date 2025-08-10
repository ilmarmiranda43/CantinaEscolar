using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CantinaEscolar.ViewModels
{
    public class VendaCreateViewModel
    {
        [StringLength(120)]
        public string? ClienteNome { get; set; }

        [StringLength(40)]
        public string? FormaPagamento { get; set; }

        public List<VendaItemInput> Itens { get; set; } = new();
    }

    public class VendaItemInput
    {
        [Required]
        public int ProdutoId { get; set; }

        [Range(1, 100000)]
        public int Quantidade { get; set; }
    }
}
