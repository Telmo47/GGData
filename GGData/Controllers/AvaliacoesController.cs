using System;
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

        // Método auxiliar para popular dropdowns no Create/Edit (se quiseres incluir tipo usuário)
        private void PopularViewData(Avaliacao avaliacao = null)
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", avaliacao?.JogoId);

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Nome", avaliacao?.UsuarioId);

            var tiposUsuario = new[] { "Crítico", "Utilizador" };
            ViewData["TipoUsuario"] = new SelectList(tiposUsuario, avaliacao?.TipoUsuario);
        }

        // GET: Avaliacaos
        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario);

            ViewBag.CurrentUserName = User.Identity.Name;

            return View(await avaliacoes.ToListAsync());
        }

        // GET: Avaliacaos/Details/5
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

        // GET: Avaliacaos/Create
        public IActionResult Create()
        {
            PopularViewData();
            return View();
        }

        // POST: Avaliacaos/Create
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

        // GET: Avaliacaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var username = User.Identity.Name;
            var isAdmin = User.IsInRole("Administrador");

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.UserName == username));

            if (avaliacao == null) return RedirectToAction(nameof(Index));

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        // POST: Avaliacaos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuarioId,JogoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId) return NotFound();

            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Administrador");

            if (avaliacao.UsuarioId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

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
            if (id == null) return RedirectToAction(nameof(Index));

            var username = User.Identity.Name;
            var isAdmin = User.IsInRole("Administrador");

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.UserName == username));

            if (avaliacao == null) return RedirectToAction(nameof(Index));

            return View(avaliacao);
        }

        // POST: Avaliacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Administrador");

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.Id == currentUserId));

            if (avaliacao == null) return RedirectToAction(nameof(Index));

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
