using Microsoft.AspNetCore.Identity;

namespace CantinaEscolar.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public string RA { get; set; } = string.Empty; // Registro do aluno
        public string PhoneNumber {  get; set; } = string.Empty;
    }
}
