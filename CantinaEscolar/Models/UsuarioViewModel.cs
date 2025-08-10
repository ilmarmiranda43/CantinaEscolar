namespace CantinaEscolar.Models
{
    public class UsuarioViewModel
    {
        public string? Id { get; set; } 
        public string? Nome { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Fone { get; set; }
        public string? RA { get; set; }

        public List<string> Roles { get; set; } = new();
    }

}
