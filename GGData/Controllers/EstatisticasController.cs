using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstatisticasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var estatisticas = _context.Estatistica.Include(e => e.Jogo);
            return View(await estatisticas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);

            if (estatistica == null) return NotFound();

            return View(estatistica);
        }

        public IActionResult Create()
        {
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstatisticaId,Conquistas,TempoMedioJogo,TotalAvaliacoes,MediaNotaUtilizadores,MediaNotaCriticos,JogoID")] Estatistica estatistica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estatistica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoID);
            return View(estatistica);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica == null) return NotFound();

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoID);
            return View(estatistica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstatisticaId,Conquistas,TempoMedioJogo,TotalAvaliacoes,MediaNotaUtilizadores,MediaNotaCriticos,JogoID")] Estatistica estatistica)
        {
            if (id != estatistica.EstatisticaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estatistica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstatisticaExists(estatistica.EstatisticaId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoID);
            return View(estatistica);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);

            if (estatistica == null) return NotFound();

            return View(estatistica);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica != null)
            {
                _context.Estatistica.Remove(estatistica);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EstatisticaExists(int id)
        {
            return _context.Estatistica.Any(e => e.EstatisticaId == id);
        }
    }
}
