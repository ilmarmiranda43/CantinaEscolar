using CantinaEscolar.Data;
using CantinaEscolar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Controllers
{
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var compras = await _context.Compras.Include(c => c.Aluno).ToListAsync();
            return View(compras);
        }

        public IActionResult Create()
        {
            ViewBag.Alunos = new SelectList(_context.Alunos, "Id", "Nome");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Compra compra)
        {
            var aluno = await _context.Alunos.FindAsync(compra.AlunoId);

            if (aluno == null)
            {
                ModelState.AddModelError("AlunoId", "Aluno não encontrado.");
                ViewBag.Alunos = new SelectList(_context.Alunos, "Id", "Nome");
                return View(compra);
            }

            if (aluno.ValorDisponivel >= compra.Valor)
            {
                aluno.ValorDisponivel -= compra.Valor;
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("", "Saldo insuficiente.");
                ViewBag.Alunos = new SelectList(_context.Alunos, "Id", "Nome");
                return View(compra);
            }

            return RedirectToAction("Index");
        }

    }
}
