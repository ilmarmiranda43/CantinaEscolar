using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CantinaEscolar.Data;
using CantinaEscolar.Services;
using CantinaEscolar.ViewModels;


namespace CantinaEscolar.Controllers
{
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IVendaService _service;

        public VendasController(ApplicationDbContext db, IVendaService service)
        {
            _db = db;
            _service = service;
        }

        // Exibe formulário
        [HttpGet]
        public async Task<IActionResult> Create(int? alunoId)
        {
            var vm = await _service.CarregarTelaAsync(alunoId);
            ViewBag.Alunos = new SelectList(await _db.Alunos.OrderBy(a => a.Nome).ToListAsync(), "Id", "Nome", vm.AlunoId);
            return View(vm);
        }

        // Posta venda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Alunos = new SelectList(await _db.Alunos.OrderBy(a => a.Nome).ToListAsync(), "Id", "Nome", vm.AlunoId);
                return View(vm);
            }

            var (ok, mensagem) = await _service.EfetivarVendaAsync(vm);
            if (!ok)
            {
                vm.MensagemErro = mensagem;

                // Recarrega dropdown e tabela de produtos
                ViewBag.Alunos = new SelectList(await _db.Alunos.OrderBy(a => a.Nome).ToListAsync(), "Id", "Nome", vm.AlunoId);

                // Recarrega dados do aluno/limite
                var recarregado = await _service.CarregarTelaAsync(vm.AlunoId);
                vm.LimiteResponsavel = recarregado.LimiteResponsavel;
                vm.TotalJaConsumidoNoMes = recarregado.TotalJaConsumidoNoMes;

                // Mantém quantidades digitadas
                foreach (var item in vm.Itens)
                {
                    var linha = recarregado.Itens.FirstOrDefault(x => x.ProdutoId == item.ProdutoId);
                    if (linha != null) { linha.Quantidade = item.Quantidade; }
                }
                vm.Itens = recarregado.Itens;

                return View(vm);
            }

            TempData["MensagemOk"] = mensagem;
            return RedirectToAction(nameof(Sucesso));
        }

        [HttpGet]
        public IActionResult Sucesso()
        {
            ViewBag.MensagemOk = TempData["MensagemOk"]?.ToString();
            return View();
        }

        // Endpoint para atualizar o "limite disponível" via AJAX ao trocar o aluno
        [HttpGet]
        public async Task<IActionResult> LimiteDisponivel(int alunoId)
        {
            var disponivel = await _service.ObterLimiteDisponivelAsync(alunoId);
            return Json(new { disponivel });
        }
    }
}
