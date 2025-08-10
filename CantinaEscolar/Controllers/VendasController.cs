using CantinaEscolar.Data;
using CantinaEscolar.Models;
using CantinaEscolar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace CantinaEscolar.Controllers
{
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VendasController(ApplicationDbContext context) => _context = context;

        // GET: /Vendas
        public async Task<IActionResult> Index()
        {
            var vendas = await _context.Vendas
                .AsNoTracking()
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            return View(vendas);
        }

        // GET: /Vendas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null) return NotFound();
            return View(venda);
        }

        // GET: /Vendas/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarProdutosAsync();
            return View(new VendaCreateViewModel
            {
                Itens = new List<VendaItemInput> { new VendaItemInput { Quantidade = 1 } }
            });
        }

        // POST: /Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaCreateViewModel vm)
        {
            // valida itens
            if (vm.Itens == null || vm.Itens.Count == 0)
                ModelState.AddModelError("", "Adicione ao menos um item à venda.");

            // remove itens inválidos (ex.: ProdutoId 0)
            vm.Itens = vm.Itens
                .Where(i => i.ProdutoId > 0 && i.Quantidade > 0)
                .ToList();

            if (!ModelState.IsValid)
            {
                await CarregarProdutosAsync();
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = new Venda
                {
                    ClienteNome = vm.ClienteNome,
                    FormaPagamento = vm.FormaPagamento,
                    DataVenda = DateTime.Now,
                    Total = 0m,
                    Itens = new List<VendaItem>()
                };

                foreach (var item in vm.Itens)
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == item.ProdutoId);
                    if (produto == null)
                    {
                        ModelState.AddModelError("", $"Produto {item.ProdutoId} não encontrado.");
                        await CarregarProdutosAsync();
                        return View(vm);
                    }

                    if (produto.Quantidade < item.Quantidade)
                    {
                        ModelState.AddModelError("", $"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.Quantidade}.");
                        await CarregarProdutosAsync();
                        return View(vm);
                    }

                    // baixa estoque
                    produto.Quantidade -= item.Quantidade;

                    var vendaItem = new VendaItem
                    {
                        ProdutoId = produto.Id,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = produto.Preco
                    };

                    venda.Itens.Add(vendaItem);
                    venda.Total += vendaItem.PrecoUnitario * vendaItem.Quantidade;
                }

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                TempData["msg"] = "Venda registrada com sucesso!";
                return RedirectToAction(nameof(Details), new { id = venda.Id });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Erro ao salvar a venda: " + ex.Message);
                await CarregarProdutosAsync();
                return View(vm);
            }
        }

        private async Task CarregarProdutosAsync()
        {
            var produtos = await _context.Produtos
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .ToListAsync();

            ViewBag.Produtos = new SelectList(produtos, "Id", "Nome");
        }
    }
}
