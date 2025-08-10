using CantinaEscolar.Data;
using CantinaEscolar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace CantinaEscolar.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProdutosController(ApplicationDbContext context) => _context = context;

        // GET: /Produtos
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Produtos.AsNoTracking().ToListAsync();
            return View(lista);
        }

        // GET: /Produtos/Create
        [HttpGet]
        public IActionResult Create() => View();

        // POST: /Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (!ModelState.IsValid) return View(produto);

            _context.Add(produto);
            await _context.SaveChangesAsync();
            TempData["msg"] = "Produto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // (Opcional: depois dá pra incluir Edit, Details, Delete)
    }
}
