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
    [Authorize] // Só utilizadores autenticados podem aceder a este controlador
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém o ID do utilizador atualmente autenticado com base no nome de utilizador.
        /// </summary>
        /// <returns>ID do utilizador atual</returns>
        private int GetCurrentUserId()
        {
            var username = User.Identity.Name;
            return _context.Utilizadores
                .Where(u => u.UserName == username)
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Preenche os dados necessários para as dropdowns no ViewData,
        /// incluindo a lista de jogos e tipos de utilizadores.
        /// </summary>
        private void PopularViewData(Avaliacao avaliacao = null)
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", avaliacao?.JogoId);
            var tiposUsuario = new[] { "Crítico", "Utilizador" };
            ViewData["TipoUsuario"] = new SelectList(tiposUsuario, avaliacao?.TipoUsuario);
        }

        /// <summary>
        /// Lista todas as avaliações, incluindo informações dos jogos e dos utilizadores.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var avaliacoes = _context.Avaliacao
                .Include(a => a.Jogo)
                .Include(a => a.Utilizador);
            return View(await avaliacoes.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de uma avaliação específica pelo seu ID.
        /// </summary>
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
        /// Se for passado um jogoId, preenche automaticamente o campo JogoId.
        /// </summary>
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
        /// Recebe a submissão do formulário para criar uma nova avaliação.
        /// Verifica se o utilizador já avaliou o jogo e adiciona a nova avaliação.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nota,Comentarios,TipoUsuario,JogoId")] Avaliacao avaliacao)
        {
            // Atribui o utilizador atual à avaliação
            avaliacao.UtilizadorId = GetCurrentUserId();

            // Verifica se já existe avaliação para este jogo e utilizador
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var avaliacao = await _context.Avaliacao.FindAsync(id);
            if (avaliacao == null) return NotFound();

            PopularViewData(avaliacao);
            return View(avaliacao);
        }

        /// <summary>
        /// Recebe a submissão do formulário para editar uma avaliação.
        /// Valida e atualiza a avaliação na base de dados.
        /// </summary>
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
        /// Exibe a página para confirmar a eliminação de uma avaliação.
        /// </summary>
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
        /// Confirma a eliminação da avaliação selecionada.
        /// </summary>
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
