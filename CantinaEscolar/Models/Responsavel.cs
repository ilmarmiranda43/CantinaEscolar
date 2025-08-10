using Microsoft.AspNetCore.Mvc;

namespace CantinaEscolar.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Responsavel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Valor para Cantina")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
        public decimal ValorParaCantina { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido.")]
        [Display(Name = "Telefone")]
        public string? Fone { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; }

        [Range(1, 31, ErrorMessage = "O dia do pagamento deve estar entre 1 e 31.")]
        [Display(Name = "Dia do Pagamento")]
        public int? DiaPgto { get; set; }


        public List<Aluno> Alunos { get; set; } = new();
    }


}
