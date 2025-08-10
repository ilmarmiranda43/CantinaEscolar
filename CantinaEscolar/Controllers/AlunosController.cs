using CantinaEscolar.Data;
using CantinaEscolar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Controllers
{
    public class AlunosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlunosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var alunos = await _context.Alunos.Include(a => a.Responsavel).ToListAsync();
            return View(alunos);
        }

        public IActionResult Create()
        {
            var lista = _context.Responsaveis
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Nome
                })
                .ToList();

            // Insere uma opção vazia no topo
            lista.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "", // ou "Selecione..." se quiser mostrar algo
                Selected = true
            });

            ViewBag.Responsaveis = lista;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Aluno aluno)
        {
            var responsavel = await _context.Responsaveis.FindAsync(aluno.ResponsavelId);

            if (responsavel == null)
            {
                ModelState.AddModelError("ResponsavelId", "Responsável não encontrado.");
                return View(aluno);
            }

            aluno.ValorDisponivel = responsavel.ValorParaCantina;

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
