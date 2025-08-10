using Microsoft.AspNetCore.Mvc;

namespace CantinaEscolar.Models
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        
        public DateTime DataNascimento { get; set; }

        public string Serie { get; set; } = string.Empty;

        public decimal ValorDisponivel { get; set; }

        public int ResponsavelId { get; set; }
        public Responsavel Responsavel { get; set; } = null!;

    }



}
