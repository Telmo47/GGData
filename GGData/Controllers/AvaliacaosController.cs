using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGData.Data;
using GGData.Models;

namespace GGData.Controllers
{
    public class AvaliacaosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacaosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void PopularViewData(Avaliacao avaliacao = null)
        {
            // Dropdown para escolher o jogo pelo nome
            ViewData["JogoId"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao?.JogoId);

            // Dropdown para escolher o usuário pelo nome
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nome", avaliacao?.UsuarioId);

            // Dropdown fixo para o tipo de usuário
            var tiposUsuario = new[] { "Crítico", "Utilizador" };
            ViewData["TipoUsuario"] = new SelectList(tiposUsuario, avaliacao?.TipoUsuario);
        }

        // GET: Avaliacaos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Avaliacao.Include(a => a.Jogo).Include(a => a.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Avaliacaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null)
                return NotFound();

            return View(avaliacao);
        }

        // GET: Avaliacaos/Create
        public IActionResult Create()
        {
            PopularViewData();
            return View();
        }

        // POST: Avaliacaos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,TipoUsuario,UsuarioId,JogoId")] Avaliacao avaliacao)
        {
            // Preenche a data automaticamente ao criar
            avaliacao.DataReview = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        // GET: Avaliacaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao == null)
                return NotFound();

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        // POST: Avaliacaos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuarioId,JogoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
                return NotFound();

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

        // GET: Avaliacaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null)
                return NotFound();

            return View(avaliacao);
        }

        // POST: Avaliacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao != null)
                _context.Avaliacao.Remove(avaliacao);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoExists(int id)
        {
            return _context.Avaliacao.Any(e => e.AvaliacaoId == id);
        }
    }
}
