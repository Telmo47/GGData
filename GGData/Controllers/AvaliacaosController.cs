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
    public class AvaliacaosController : Controller
    {

        /// <summary>
        /// referência à base de dados
        /// </summary>
        private readonly ApplicationDbContext _context;

        public AvaliacaosController(ApplicationDbContext context)
        {
            _context = context;
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
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // GET: Avaliacaos/Create
        private void PopularViewData(Avaliacao avaliacao = null)
        {
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", avaliacao?.JogoId);
            ViewData["UsuariosID"] = new SelectList(_context.Usuarios, "UsuarioId", "Nome", avaliacao?.UsuariosId);
        }

        // GET: Avaliacaos/Create
        public IActionResult Create()
        {
            PopularViewData();
            return View();
        }

        // POST: Avaliacaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,DataReview,TipoUsuario,UsuariosID,JogoID")]
 Avaliacao avaliacao)
        {

            // Tarefas
            // - ajustar o nome das variáveis
            // - ajustar os anotadores, neste caso em concreto,
            //    eliminar o ID do 'Bind'

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
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "JogoId", avaliacao.JogoId);
            ViewData["UsuariosID"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", avaliacao.UsuariosId);
            return View(avaliacao);
        }

        // POST: Avaliacaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentarios,DataReview,TipoUsuario,UsuariosID,JogoID")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
            {
                return NotFound();
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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "JogoId", avaliacao.JogoId);
            ViewData["UsuariosID"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", avaliacao.UsuariosId);
            return View(avaliacao);
        }

        // GET: Avaliacaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // POST: Avaliacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao != null)
            {
                _context.Avaliacao.Remove(avaliacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoExists(int id)
        {
            return _context.Avaliacao.Any(e => e.AvaliacaoId == id);
        }
    }
}
