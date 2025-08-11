using System.ComponentModel.DataAnnotations;

namespace CantinaEscolar.ViewModels
{
    public class VendaItemVM
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = "";
        public decimal Preco { get; set; }

        [Range(0, 9999)]
        public int Quantidade { get; set; }
    }
}
