using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CantinaEscolar.ViewModels
{
    public class VendaViewModel
    {
        [Display(Name = "Aluno")]
        [Required]
        public int AlunoId { get; set; }

        public string AlunoNome { get; set; } = "";

        // Tabela de produtos para o formulário (linha com quantidade)
        public List<VendaItemVM> Itens { get; set; } = new();

        public decimal LimiteResponsavel { get; set; }
        public decimal TotalJaConsumidoNoMes { get; set; }
        public decimal LimiteDisponivel => LimiteResponsavel - TotalJaConsumidoNoMes;

        public decimal TotalDaVenda { get; set; }

        public string? MensagemErro { get; set; }
        public string? MensagemOk { get; set; }
    }
}
