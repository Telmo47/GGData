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
    /// <summary>
    /// Controlador responsável por gerir as estatísticas dos jogos.
    /// Inclui operações CRUD para Estatística, que está associada a um Jogo.
    /// </summary>
    public class EstatisticasController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da aplicação para acesso à base de dados.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework</param>
        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as estatísticas dos jogos com os dados do jogo incluídos.
        /// </summary>
        /// <returns>View com lista de Estatísticas</returns>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Estatistica.Include(e => e.Jogo);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de uma estatística específica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <returns>View com detalhes ou NotFound se não existir</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);
            if (estatistica == null)
            {
                return NotFound();
            }

            return View(estatistica);
        }

        /// <summary>
        /// Apresenta o formulário para criar uma nova estatística.
        /// </summary>
        /// <returns>View de criação</returns>
        public IActionResult Create()
        {
            // Popula dropdown com jogos existentes pelo seu nome para melhor usabilidade
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome");
            return View();
        }

        /// <summary>
        /// Processa a criação da nova estatística.
        /// </summary>
        /// <param name="estatistica">Objeto Estatística preenchido</param>
        /// <returns>Redireciona para Index ou mostra erros</returns>
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
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Apresenta o formulário para editar uma estatística existente.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <returns>View de edição ou NotFound</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica == null)
            {
                return NotFound();
            }
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Processa a edição da estatística.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <param name="estatistica">Objeto Estatística com dados atualizados</param>
        /// <returns>Redireciona para Index ou mostra erros</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstatisticaId,Conquistas,TempoMedioJogo,TotalAvaliacoes,MediaNotaUtilizadores,MediaNotaCriticos,JogoID")] Estatistica estatistica)
        {
            if (id != estatistica.EstatisticaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estatistica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstatisticaExists(estatistica.EstatisticaId))
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
            ViewData["JogoID"] = new SelectList(_context.Jogo, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Apresenta a confirmação para eliminar uma estatística.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <returns>View de confirmação ou NotFound</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);
            if (estatistica == null)
            {
                return NotFound();
            }

            return View(estatistica);
        }

        /// <summary>
        /// Processa a eliminação da estatística após confirmação.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <returns>Redireciona para Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica != null)
            {
                _context.Estatistica.Remove(estatistica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma estatística existe pelo seu ID.
        /// </summary>
        /// <param name="id">ID da Estatística</param>
        /// <returns>True se existir, False caso contrário</returns>
        private bool EstatisticaExists(int id)
        {
            return _context.Estatistica.Any(e => e.EstatisticaId == id);
        }
    }
}
