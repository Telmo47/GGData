using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Lista todos os jogos, com opção de filtrar por género.
        /// </summary>
        /// <param name="genero">Género a pesquisar (opcional).</param>
        /// <returns>View com lista de jogos filtrados (ou todos se vazio).</returns>
        public async Task<IActionResult> Index(string genero)
        {
            // Consulta base (todos os jogos)
            var jogos = from j in _context.Jogo select j;

            // Aplica filtro se houver género indicado
            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.Genero.Contains(genero));
            }

            // Envia os resultados para a view
            return View(await jogos.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de um jogo específico.
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo.FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null) return NotFound();

            return View(jogo);
        }

        /// <summary>
        /// Formulário de criação de novo jogo.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Recebe e guarda os dados de um novo jogo.
        /// </summary>
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

        /// <summary>
        /// Formulário para editar um jogo existente.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo.FindAsync(id);
            if (jogo == null) return NotFound();

            return View(jogo);
        }

        /// <summary>
        /// Recebe alterações de um jogo e guarda no banco de dados.
        /// </summary>
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

        /// <summary>
        /// Mostra confirmação para eliminar jogo.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogo.FirstOrDefaultAsync(m => m.JogoId == id);
            if (jogo == null) return NotFound();

            return View(jogo);
        }

        /// <summary>
        /// Confirma e elimina jogo.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogo.FindAsync(id);
            if (jogo != null)
            {
                _context.Jogo.Remove(jogo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um jogo com o ID especificado existe.
        /// </summary>
        private bool JogoExists(int id)
        {
            return _context.Jogo.Any(e => e.JogoId == id);
        }
    }
}

