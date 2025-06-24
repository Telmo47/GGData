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
    public class AvaliacaosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacaosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista todas as avaliações
        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario);

            // Obter o userId do utilizador autenticado para uso na view
            var username = User.Identity.Name;
            var userId = _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.UsuarioId)
                .FirstOrDefault();

            ViewBag.CurrentUserId = userId;

            return View(await avaliacoes.ToListAsync());
        }

        // Detalhes avaliação
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

        // Create (GET)
        public IActionResult Create()
        {
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome");
            return View();
        }

        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,DataReview,TipoUsuario,JogoId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var userId = _context.Usuarios
                    .Where(u => u.UserName == username)
                    .Select(u => u.UsuarioId)
                    .FirstOrDefault();

                avaliacao.UsuariosID = userId;

                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
            return View(avaliacao);
        }

        // Edit (GET) - só autor ou admin pode editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null) return NotFound();

            var username = User.Identity.Name;
            var userId = _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.UsuarioId)
                .FirstOrDefault();

            if (avaliacao.UsuariosID != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
            return View(avaliacao);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuariosID,JogoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId) return NotFound();

            var username = User.Identity.Name;
            var userId = _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.UsuarioId)
                .FirstOrDefault();

            if (avaliacao.UsuariosID != userId && !User.IsInRole("Administrador"))
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
                    if (!AvaliacaoExists(avaliacao.AvaliacaoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
            return View(avaliacao);
        }

        // Delete (GET) - só autor ou admin
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null) return NotFound();

            var username = User.Identity.Name;
            var userId = _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.UsuarioId)
                .FirstOrDefault();

            if (avaliacao.UsuariosID != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            return View(avaliacao);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            var username = User.Identity.Name;
            var userId = _context.Usuarios
                .Where(u => u.UserName == username)
                .Select(u => u.UsuarioId)
                .FirstOrDefault();

            if (avaliacao == null) return NotFound();

            if (avaliacao.UsuariosID != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

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
