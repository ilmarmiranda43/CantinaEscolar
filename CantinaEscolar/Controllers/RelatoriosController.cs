using CantinaEscolar.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SaldoPorAluno()
        {
            var alunos = await _context.Alunos.Include(a => a.Responsavel).ToListAsync();
            return View(alunos);
        }
    }
}
