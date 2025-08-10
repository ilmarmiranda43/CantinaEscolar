using CantinaEscolar.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Produto> Produtos => Set<Produto>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }
}
