using CantinaEscolar.Data;
using CantinaEscolar.Models;
using CantinaEscolar.Services;
using CantinaEscolar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CantinaEscolar.Controllers
{
    [Authorize]
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVendaService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendasController(ApplicationDbContext db, IVendaService service, UserManager<ApplicationUser> userManager)
        {
            _db = db; _service = service; _userManager = userManager;
        }

        private async Task<int?> GetAlunoIdDoUsuarioAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            var aluno = await _db.Alunos.Where(a => a.ApplicationUserId == user.Id)
                                        .Select(a => new { a.Id })
                                        .FirstOrDefaultAsync();
            return aluno?.Id;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? alunoId)
        {
            // Se for aluno, força o AlunoId do usuário
            var meuAlunoId = await GetAlunoIdDoUsuarioAsync();
            if (meuAlunoId.HasValue) alunoId = meuAlunoId;

            var vm = await _service.CarregarTelaAsync(alunoId);
            await CarregarCombosAsync(alunoId); // abaixo ajustamos para entregar só o próprio aluno
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel vm, string? addItem, int? removeItem)
        {
            // Força o AlunoId se for aluno
            var meuAlunoId = await GetAlunoIdDoUsuarioAsync();
            if (meuAlunoId.HasValue) vm.AlunoId = (int)meuAlunoId;

            // Adicionar linha vazia
            if (!string.IsNullOrEmpty(addItem))
            {
                vm.Itens ??= new List<VendaItemVM>();
                vm.Itens.Add(new VendaItemVM());     // ProdutoId = null, Quantidade = 1
                await CarregarCombosAsync(vm.AlunoId);

                // IMPORTANTE: limpa ModelState para o placeholder aparecer,
                // senão o TagHelper pode reaproveitar valores antigos.
                ModelState.Clear();
                return View(vm);
            }

            // Remover linha (pelo índice)
            if (removeItem.HasValue && removeItem.Value >= 0 && removeItem.Value < vm.Itens.Count)
            {
                vm.Itens.RemoveAt(removeItem.Value);
                await CarregarCombosAsync(vm.AlunoId);
                ModelState.Clear();
                return View(vm);
            }

            // Fluxo normal de salvar
            if (!ModelState.IsValid)
            {
                await CarregarCombosAsync(vm.AlunoId);
                return View(vm);
            }

            var (ok, msg) = await _service.EfetivarVendaAsync(vm);
            if (!ok)
            {
                vm.MensagemErro = msg;
                var rec = await _service.CarregarTelaAsync(vm.AlunoId);
                vm.LimiteResponsavel = rec.LimiteResponsavel;
                vm.TotalJaConsumidoNoMes = rec.TotalJaConsumidoNoMes;

                await CarregarCombosAsync(vm.AlunoId);
                return View(vm);
            }

            TempData["MensagemOk"] = "Venda registrada com sucesso.";
            return RedirectToAction(nameof(Sucesso));
        }


        [HttpGet]
        public IActionResult Sucesso()
        {
            ViewBag.MensagemOk = TempData["MensagemOk"]?.ToString();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LimiteDisponivel(int alunoId)
        {
            var disponivel = await _service.ObterLimiteDisponivelAsync(alunoId);
            return Json(new { disponivel });
        }

        [HttpGet]
        public async Task<IActionResult> LimiteInfo(int alunoId)
        {
            
            // Bloqueia acesso a aluno alheio
            var meuAlunoId = await GetAlunoIdDoUsuarioAsync();
            if (meuAlunoId.HasValue && alunoId != meuAlunoId.Value)
                return Forbid();

            var aluno = await _db.Alunos
                .Include(a => a.Responsavel)
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                return Json(new { limite = 0m, consumidoMes = 0m, disponivel = 0m });

            var hoje = DateTime.Now;
            var consumidoMes = await _db.Compras
                .Where(c => c.AlunoId == aluno.Id
                            && c.Data.Month == hoje.Month
                            && c.Data.Year == hoje.Year)
                .SumAsync(c => (decimal?)c.ValorTotal) ?? 0m;

            var limite = aluno.Responsavel?.ValorParaCantina ?? 0m;
            var disponivel = Math.Max(0m, limite - consumidoMes);

            Console.Write("Valores retornados Limite : " + limite);


            return Json(new { limite, consumidoMes, disponivel });
        }


        private async Task CarregarCombosAsync(int? alunoIdSelecionado)
        {
            // Se é aluno, só carrega o próprio
            var meuAlunoId = await GetAlunoIdDoUsuarioAsync();
            if (meuAlunoId.HasValue)
            {
                var meuAluno = await _db.Alunos.Where(a => a.Id == meuAlunoId.Value)
                                               .OrderBy(a => a.Nome)
                                               .ToListAsync();
                ViewBag.Alunos = new SelectList(meuAluno, "Id", "Nome", meuAlunoId.Value);
            }
            else
            {
                ViewBag.Alunos = new SelectList(await _db.Alunos.OrderBy(a => a.Nome).ToListAsync(), "Id", "Nome", alunoIdSelecionado);
            }

            var produtos = await _db.Produtos.OrderBy(p => p.Nome).Select(p => new { p.Id, p.Nome, p.Preco }).ToListAsync();
            ViewBag.Produtos = new SelectList(produtos, "Id", "Nome");
            ViewBag.ProdutosJson = System.Text.Json.JsonSerializer.Serialize(produtos); // use camelCase se seu JS espera
        }
    }
}
