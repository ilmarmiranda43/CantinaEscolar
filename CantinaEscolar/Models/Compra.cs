using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantinaEscolar.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; } = default!;

        public DateTime Data { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }   // <-- NOVO

        public ICollection<CompraItem> Itens { get; set; } = new List<CompraItem>(); // <-- NOVO
    }
}
