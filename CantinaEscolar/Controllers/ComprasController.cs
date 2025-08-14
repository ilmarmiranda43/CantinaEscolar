using CantinaEscolar.Data;
using CantinaEscolar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Controllers
{
    [Authorize]
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComprasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context; _userManager = userManager;
        }

        private async Task<string?> GetUserIdAsync() => (await _userManager.GetUserAsync(User))?.Id;

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserIdAsync();
            var query = _context.Compras.Include(c => c.Aluno);

            // Se usuário é aluno, mostra só as compras dele
            var isAluno = await _context.Alunos.AnyAsync(a => a.ApplicationUserId == userId);
            if (isAluno)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Compra, Aluno>)query.Where(c => c.Aluno.ApplicationUserId == userId);
            }

            var lista = await query.OrderByDescending(c => c.Data).ToListAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = await GetUserIdAsync();
            var compra = await _context.Compras
                .Include(c => c.Aluno)
                .Include(c => c.Itens).ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();

            var isAluno = await _context.Alunos.AnyAsync(a => a.ApplicationUserId == userId);
            if (isAluno && compra.Aluno.ApplicationUserId != userId)
                return Forbid();

            return View(compra);
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

            if (aluno.ValorDisponivel >= compra.ValorTotal)
            {
                aluno.ValorDisponivel -= compra.ValorTotal;
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
