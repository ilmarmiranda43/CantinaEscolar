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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SaldoPorAluno()
        {
            var hoje = DateTime.Now;

            var alunos = await _context.Alunos
                .Include(a => a.Responsavel)
                .OrderBy(a => a.Nome)
                .Select(a => new SaldoPorAlunoVM
                {
                    Aluno = a.Nome,
                    Responsavel = a.Responsavel.Nome,
                    Serie = a.Serie,
                    Limite = a.Responsavel.ValorParaCantina,
                    ConsumidoMes = _context.Compras
                        .Where(c => c.AlunoId == a.Id
                            && c.Data.Month == hoje.Month
                            && c.Data.Year == hoje.Year)
                        .Sum(c => (decimal?)c.ValorTotal) ?? 0m
                })
                .ToListAsync();

            foreach (var aluno in alunos)
            {
                aluno.Disponivel = Math.Max(0m, aluno.Limite - aluno.ConsumidoMes);
            }

            return View(alunos);
        }

        public async Task<IActionResult> DetalhesComprasAluno(int alunoId)
        {
            var aluno = await _context.Alunos
                .Include(a => a.Responsavel)
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                return NotFound();

            var compras = await _context.Compras
                .Include(c => c.Aluno)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(c => c.AlunoId == alunoId)
                .OrderByDescending(c => c.Data)
                .ToListAsync();

            ViewBag.Aluno = aluno;

            return View(compras);
        }
    }

    public class SaldoPorAlunoVM
    {
        public string Aluno { get; set; } = string.Empty;
        public int AlunoId { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public decimal Limite { get; set; }
        public decimal ConsumidoMes { get; set; }
        public decimal Disponivel { get; set; }
    }
}