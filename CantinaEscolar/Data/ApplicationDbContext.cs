using CantinaEscolar.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }
        public DbSet<Compra> Compras { get; set; }

        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<CompraItem> CompraItens { get; set; }  // novo
        public ICollection<CompraItem> Itens { get; set; } = new List<CompraItem>();


        public DbSet<Venda> Vendas => Set<Venda>();
        public DbSet<VendaItem> VendasItens => Set<VendaItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Venda (1) -> (N) VendaItem
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(vi => vi.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Produto (1) -> (N) VendaItem
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Produto)
                .WithMany() // se quiser, crie ICollection<VendaItem> em Produto e troque aqui
                .HasForeignKey(vi => vi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precisões decimais (se preferir garantir via Fluent API também)
            modelBuilder.Entity<Produto>().Property(p => p.Preco).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Venda>().Property(v => v.Total).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VendaItem>().Property(vi => vi.PrecoUnitario).HasColumnType("decimal(18,2)");
        }
    }
}
