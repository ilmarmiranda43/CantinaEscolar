using System.ComponentModel.DataAnnotations;

namespace CantinaEscolar.ViewModels
{
    public class VendaItemVM
    {
        [Required]
        public int ProdutoId { get; set; }

        public string ProdutoNome { get; set; } = "";

        public decimal Preco { get; set; }   // será preenchido via JS conforme o produto escolhido

        [Range(1, 9999)]
        public int Quantidade { get; set; } = 1;
    }
}
