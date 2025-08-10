using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantinaEscolar.Models
{
    public class Venda
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [StringLength(120)]
        public string? ClienteNome { get; set; }

        [StringLength(40)]
        public string? FormaPagamento { get; set; } // Ex.: Dinheiro, Cartão, PIX

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public List<VendaItem> Itens { get; set; } = new();
    }
}
