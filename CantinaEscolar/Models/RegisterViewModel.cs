using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CantinaEscolar.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O RA é obrigatório.")]
        public string RA { get; set; } = null!;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Informe o telefone.")]
        [Phone(ErrorMessage = "Número de telefone inválido.")]
        public string PhoneNumber { get; set; } = null!;


        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirme a senha.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Selecione um papel.")]
        public string Role { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();


    }

}
