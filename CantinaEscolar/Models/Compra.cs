using Microsoft.AspNetCore.Mvc;

namespace CantinaEscolar.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }

        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; }
    }

}
