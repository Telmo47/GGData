using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Para HttpContext.Session
using GGData.Data;
using GGData.Models;

namespace GGData.Controllers
{
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string genero)
        {
            var jogos = from j in _context.Jogo select j;

            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.Genero.Contains(genero));
            }

            return View(await jogos.ToListAsync());
        }

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

            // Guardar na sessão o ID do jogo a editar e ação
            HttpContext.Session.SetInt32("JogoID", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Edit");

            return View(jogo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JogoId,Nome,Genero,Plataforma,DataLancamento")] Jogo jogo)
        {
            if (id != jogo.JogoId) return NotFound();

            var jogoIDSessao = HttpContext.Session.GetInt32("JogoID");
            var acao = HttpContext.Session.GetString("Acao");

            if (jogoIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o jogo. Tem de reiniciar o processo.");
                return View(jogo);
            }

            if (jogoIDSessao != jogo.JogoId || acao != "Jogos/Edit")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove("JogoID");
                    HttpContext.Session.Remove("Acao");
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

            var jogo = await _context.Jogo.FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null) return NotFound();

            // Guardar na sessão o ID do jogo a eliminar e ação
            HttpContext.Session.SetInt32("JogoID", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Delete");

            return View(jogo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogo.FindAsync(id);

            var jogoIDSessao = HttpContext.Session.GetInt32("JogoID");
            var acao = HttpContext.Session.GetString("Acao");

            if (jogoIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar o jogo. Tem de reiniciar o processo.");
                return View(jogo);
            }

            if (jogoIDSessao != id || acao != "Jogos/Delete")
            {
                return RedirectToAction("Index");
            }

            if (jogo != null)
            {
                _context.Jogo.Remove(jogo);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("JogoID");
                HttpContext.Session.Remove("Acao");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JogoExists(int id)
        {
            return _context.Jogo.Any(e => e.JogoId == id);
        }
    }
}
