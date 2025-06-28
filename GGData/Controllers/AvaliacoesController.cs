using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    [Authorize]
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var username = User.Identity.Name;
            return _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        private void PopularViewData(Avaliacao avaliacao = null)
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", avaliacao?.JogoId);
            var tiposUsuario = new[] { "Crítico", "Utilizador" };
            ViewData["TipoUsuario"] = new SelectList(tiposUsuario, avaliacao?.TipoUsuario);
        }

        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario);
            return View(await avaliacoes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null) return NotFound();

            return View(avaliacao);
        }

        public IActionResult Create()
        {
            PopularViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,TipoUsuario,JogoId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                avaliacao.DataReview = DateTime.Now;
                avaliacao.UsuarioId = GetCurrentUserId();

                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null) return NotFound();

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuarioId,JogoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Avaliacao.Any(e => e.AvaliacaoId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null) return NotFound();

            return View(avaliacao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
