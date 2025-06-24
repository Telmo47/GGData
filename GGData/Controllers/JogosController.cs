using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GGData.Data;
using GGData.Models;

namespace GGData.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string genero)
        {
            var jogos = _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.Genero.Contains(genero));
            }

            return View(await jogos.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo.FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null) return NotFound();

            return View(jogo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JogoId,Nome,Genero,Plataforma,DataLancamento")] Jogo jogo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo.FindAsync(id);
            if (jogo == null) return NotFound();

            return View(jogo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JogoId,Nome,Genero,Plataforma,DataLancamento")] Jogo jogo)
        {
            if (id != jogo.JogoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogoExists(jogo.JogoId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(m => m.JogoId == id);

            if (jogo == null) return NotFound();

            return View(jogo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(j => j.JogoId == id);

            if (jogo != null && jogo.Avaliacoes.Count == 0 && jogo.Estatisticas.Count == 0)
            {
                _context.Jogo.Remove(jogo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JogoExists(int id)
        {
            return _context.Jogo.Any(e => e.JogoId == id);
        }
    }
}
