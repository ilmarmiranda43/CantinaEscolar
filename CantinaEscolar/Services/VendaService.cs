using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CantinaEscolar.Data;
using CantinaEscolar.Models;
using CantinaEscolar.ViewModels;

namespace CantinaEscolar.Services
{
    public class VendaService : IVendaService
    {
        private readonly ApplicationDbContext _db;

        public VendaService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<VendaViewModel> CarregarTelaAsync(int? alunoId = null)
        {
            var vm = new VendaViewModel(); // NÃO preencher vm.Itens aqui

            if (alunoId.HasValue)
            {
                var aluno = await _db.Alunos
                    .Include(a => a.Responsavel)
                    .FirstOrDefaultAsync(a => a.Id == alunoId.Value);

                if (aluno != null)
                {
                    vm.AlunoId = aluno.Id;
                    vm.AlunoNome = aluno.Nome;

                    var hoje = DateTime.Now;
                    var totalMes = await _db.Compras
                        .Where(c => c.AlunoId == aluno.Id
                                    && c.Data.Month == hoje.Month
                                    && c.Data.Year == hoje.Year)
                        .SumAsync(c => (decimal?)c.ValorTotal) ?? 0m;

                    vm.TotalJaConsumidoNoMes = totalMes;
                    vm.LimiteResponsavel = aluno.Responsavel?.ValorParaCantina ?? 0m;
                }
            }

            // vm.Itens fica vazio; as linhas vêm do botão "Adicionar item"
            return vm;
        }


        public async Task<(bool ok, string mensagem)> EfetivarVendaAsync(VendaViewModel vm)
        {
            // Carrega aluno + responsável
            var aluno = await _db.Alunos
                .Include(a => a.Responsavel)
                .FirstOrDefaultAsync(a => a.Id == vm.AlunoId);

            if (aluno == null)
                return (false, "Aluno não encontrado.");

            var hoje = DateTime.Now;
            var totalMes = await _db.Compras
                .Where(c => c.AlunoId == aluno.Id
                            && c.Data.Month == hoje.Month
                            && c.Data.Year == hoje.Year)
                .SumAsync(c => (decimal?)c.ValorTotal) ?? 0m;

            var limite = aluno.Responsavel?.ValorParaCantina ?? 0m;

            // Total da venda
            var itensSelecionados = vm.Itens.Where(i => i.Quantidade > 0).ToList();
            if (!itensSelecionados.Any())
                return (false, "Selecione ao menos 1 item com quantidade.");

            var ids = itensSelecionados.Select(i => i.ProdutoId).ToList();
            var produtos = await _db.Produtos.Where(p => ids.Contains(p.Id)).ToListAsync();

            decimal totalVenda = 0m;
            foreach (var item in itensSelecionados)
            {
                var prod = produtos.First(p => p.Id == item.ProdutoId);
                totalVenda += item.Quantidade * prod.Preco;
            }

            // Valida limite mensal (limite do responsável - consumido no mês)
            var disponivel = limite - totalMes;
            if (totalVenda > disponivel)
                return (false, $"Limite insuficiente. Disponível: {disponivel:C2} | Total da venda: {totalVenda:C2}");

            // Efetiva em transação
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var compra = new Compra
                {
                    AlunoId = aluno.Id,
                    Data = DateTime.Now,
                    ValorTotal = totalVenda,
                    Itens = itensSelecionados.Select(i =>
                    {
                        var prod = produtos.First(p => p.Id == i.ProdutoId);
                        return new CompraItem
                        {
                            ProdutoId = prod.Id,
                            Quantidade = i.Quantidade,
                            PrecoUnitario = prod.Preco
                        };
                    }).ToList()
                };

                _db.Compras.Add(compra);

                // Se seu Produto tiver campo de estoque, você pode decrementar aqui.
                // foreach (var i in itensSelecionados)
                // {
                //     var prod = produtos.First(p => p.Id == i.ProdutoId);
                //     prod.Estoque -= i.Quantidade;
                // }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return (true, "Venda registrada com sucesso.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, $"Erro ao registrar venda: {ex.Message}");
            }
        }

        public async Task<decimal> ObterLimiteDisponivelAsync(int alunoId)
        {
            var aluno = await _db.Alunos.Include(a => a.Responsavel)
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null) return 0m;

            var hoje = DateTime.Now;
            var totalMes = await _db.Compras
                .Where(c => c.AlunoId == aluno.Id
                            && c.Data.Month == hoje.Month
                            && c.Data.Year == hoje.Year)
                .SumAsync(c => (decimal?)c.ValorTotal) ?? 0m;

            var limite = aluno.Responsavel?.ValorParaCantina ?? 0m;
            return Math.Max(0m, limite - totalMes);
        }
    }
}
