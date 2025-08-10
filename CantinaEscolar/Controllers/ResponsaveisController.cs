using Microsoft.AspNetCore.Mvc;
using CantinaEscolar.Models; // <- ajuste conforme seu namespace
using CantinaEscolar.Data;   // <- onde está seu DbContext
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CantinaEscolar.Controllers
{
    public class ResponsaveisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsaveisController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Responsaveis.ToListAsync();
            return View(lista);
        }

        // GET: Responsaveis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Responsaveis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Responsavel responsavel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(responsavel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(responsavel);
        }

        // GET: Responsaveis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var responsavel = await _context.Responsaveis.FindAsync(id);
            if (responsavel == null)
                return NotFound();

            return View(responsavel);
        }

        // POST: Responsaveis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Responsavel responsavel)
        {
            if (id != responsavel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(responsavel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Responsaveis.Any(e => e.Id == responsavel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(responsavel);
        }

        // GET: Responsaveis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var responsavel = await _context.Responsaveis
                .FirstOrDefaultAsync(m => m.Id == id);

            if (responsavel == null)
                return NotFound();

            return View(responsavel);
        }

        // POST: Responsaveis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var responsavel = await _context.Responsaveis.FindAsync(id);
            if (responsavel != null)
            {
                _context.Responsaveis.Remove(responsavel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


    }

}
