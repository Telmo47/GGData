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
    /// <summary>
    /// Controlador MVC para gerir avaliações de jogos.
    /// Apenas utilizadores autenticados podem aceder.
    /// </summary>
    [Authorize]
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da aplicação</param>
        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém o ID do utilizador autenticado atual.
        /// </summary>
        /// <returns>ID do utilizador ou 0 se não encontrado</returns>
        private int GetCurrentUserId()
        {
            var username = User.Identity.Name;
            return _context.Utilizadores
                .Where(u => u.UserName == username)
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Popula os dados para as dropdown lists na View.
        /// </summary>
        /// <param name="avaliacao">Avaliação atual para pré-selecionar campos</param>
        private void PopularViewData(Avaliacao avaliacao = null)
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", avaliacao?.JogoId);
            var tiposUsuario = new[] { "Crítico", "Utilizador" };
            ViewData["TipoUsuario"] = new SelectList(tiposUsuario, avaliacao?.TipoUsuario);
        }

        /// <summary>
        /// Lista todas as avaliações com dados dos jogos e utilizadores.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Utilizador);
            return View(await avaliacoes.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de uma avaliação específica.
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Utilizador)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null) return NotFound();

            return View(avaliacao);
        }

        /// <summary>
        /// Exibe o formulário para criar uma nova avaliação.
        /// </summary>
        /// <param name="jogoId">ID do jogo para pré-selecionar</param>
        public IActionResult Create(int? jogoId)
        {
            var avaliacao = new Avaliacao();
            if (jogoId.HasValue)
            {
                avaliacao.JogoId = jogoId.Value;
            }

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        /// <summary>
        /// Recebe os dados submetidos para criar uma nova avaliação.
        /// </summary>
        /// <param name="avaliacao">Dados da avaliação</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,TipoUsuario,JogoId")] Avaliacao avaliacao)
        {
            avaliacao.UtilizadorId = GetCurrentUserId();

            // Verifica se o utilizador já avaliou este jogo
            var existeAvaliacao = await _context.Avaliacao.AnyAsync(a =>
                a.JogoId == avaliacao.JogoId && a.UtilizadorId == avaliacao.UtilizadorId);

            if (existeAvaliacao)
            {
                ModelState.AddModelError("", "Já avaliou este jogo anteriormente.");
                PopularViewData(avaliacao);
                return View(avaliacao);
            }

            if (ModelState.IsValid)
            {
                avaliacao.DataReview = DateTime.Now;
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        /// <summary>
        /// Exibe o formulário para editar uma avaliação existente.
        /// </summary>
        /// <param name="id">ID da avaliação a editar</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null) return NotFound();

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        /// <summary>
        /// Recebe os dados para atualizar uma avaliação existente.
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <param name="avaliacao">Dados atualizados da avaliação</param>
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

        /// <summary>
        /// Exibe confirmação para apagar uma avaliação.
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Utilizador)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);

            if (avaliacao == null) return NotFound();

            return View(avaliacao);
        }

        /// <summary>
        /// Apaga a avaliação confirmada.
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao != null)
            {
                _context.Avaliacao.Remove(avaliacao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
