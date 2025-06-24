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

<<<<<<< HEAD
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
=======
        private int GetCurrentUserId()
>>>>>>> Autentication
        {
            var username = User.Identity.Name;
            return _context.Usuarios.Where(u => u.UserName == username).Select(u => u.UsuarioId).FirstOrDefault();
        }

        // Lista todas as avaliações
        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario);

            ViewBag.CurrentUserName = User.Identity.Name;

            return View(await avaliacoes.ToListAsync());
        }


        // Detalhes avaliação
        public async Task<IActionResult> Details(int? id)
        {
<<<<<<< HEAD
            if (id == null)
                return NotFound();
=======
            if (id == null) return NotFound();
>>>>>>> Autentication

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

<<<<<<< HEAD
            if (avaliacao == null)
                return NotFound();
=======
            if (avaliacao == null) return NotFound();
>>>>>>> Autentication

            return View(avaliacao);
        }

<<<<<<< HEAD
        // GET: Avaliacaos/Create
=======
        // Create (GET)
>>>>>>> Autentication
        public IActionResult Create()
        {
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome");
            return View();
        }

<<<<<<< HEAD
        // POST: Avaliacaos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,TipoUsuario,UsuarioId,JogoId")] Avaliacao avaliacao)
        {
            // Preenche a data automaticamente ao criar
            avaliacao.DataReview = DateTime.Now;

=======
        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,DataReview,TipoUsuario,JogoId")] Avaliacao avaliacao)
        {
>>>>>>> Autentication
            if (ModelState.IsValid)
            {
                avaliacao.UsuarioId = GetCurrentUserId();

                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
<<<<<<< HEAD

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
=======

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
            return View(avaliacao);
        }

        // Edit (GET) - só dono ou admin pode editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var username = User.Identity.Name;
            var isAdmin = User.IsInRole("Administrador");

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.UserName == username));

            if (avaliacao == null) return RedirectToAction(nameof(Index));

            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
            return View(avaliacao);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuariosID,JogoId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId) return NotFound();

            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Administrador");

            if (avaliacao.UsuarioId != currentUserId && !isAdmin)
            {
                return Forbid();
            }
>>>>>>> Autentication

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
<<<<<<< HEAD
                    if (!_context.Avaliacao.Any(e => e.AvaliacaoId == id))
=======
                    if (!AvaliacaoExists(avaliacao.AvaliacaoId))
>>>>>>> Autentication
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

<<<<<<< HEAD
            PopularViewData(avaliacao);
=======
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao.JogoId);
>>>>>>> Autentication
            return View(avaliacao);
        }

        // Delete (GET) - só dono ou admin
        public async Task<IActionResult> Delete(int? id)
        {
<<<<<<< HEAD
            if (id == null)
                return NotFound();
=======
            if (id == null) return RedirectToAction(nameof(Index));

            var username = User.Identity.Name;
            var isAdmin = User.IsInRole("Administrador");
>>>>>>> Autentication

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
<<<<<<< HEAD
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null)
                return NotFound();
=======
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.UserName == username));

            if (avaliacao == null) return RedirectToAction(nameof(Index));
>>>>>>> Autentication

            return View(avaliacao);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
<<<<<<< HEAD
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao != null)
                _context.Avaliacao.Remove(avaliacao);
=======
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Administrador");
>>>>>>> Autentication

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.AvaliacaoId == id && (isAdmin || a.Usuario.UsuarioId == currentUserId));

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
