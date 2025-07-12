using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    [Authorize(Roles = "Administrador")] // Apenas administradores podem aceder à maioria das ações
    public class EstatisticasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as estatísticas com os respetivos jogos.
        /// Esta ação é permitida a utilizadores anónimos.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var estatisticas = _context.Estatistica.Include(e => e.Jogo);
            return View(await estatisticas.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de uma estatística específica pelo seu ID.
        /// Esta ação é permitida a utilizadores anónimos.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);

            if (estatistica == null)
                return NotFound();

            return View(estatistica);
        }

        /// <summary>
        /// Mostra detalhes de uma estatística dado o ID do jogo (chave estrangeira).
        /// Reutiliza a view Details.
        /// Esta ação é permitida a utilizadores anónimos.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> DetailsByJogo(int? jogoId)
        {
            if (jogoId == null)
                return NotFound();

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(e => e.JogoId == jogoId);

            if (estatistica == null)
                return NotFound();

            return View("Details", estatistica);
        }

        /// <summary>
        /// Exibe o formulário para criar uma nova estatística.
        /// </summary>
        public IActionResult Create()
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome");
            return View();
        }

        /// <summary>
        /// Recebe a submissão do formulário para criar uma nova estatística.
        /// Valida e adiciona a estatística à base de dados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstatisticaId,Conquistas,TempoMedioJogo,TotalAvaliacoes,MediaNotaUtilizadores,MediaNotaCriticos,JogoId")] Estatistica estatistica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estatistica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Exibe o formulário para editar uma estatística existente.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica == null)
                return NotFound();

            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Recebe a submissão do formulário para editar uma estatística.
        /// Valida e atualiza a estatística na base de dados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstatisticaId,Conquistas,TempoMedioJogo,TotalAvaliacoes,MediaNotaUtilizadores,MediaNotaCriticos,JogoId")] Estatistica estatistica)
        {
            if (id != estatistica.EstatisticaId)
                return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome", estatistica.JogoId);
            return View(estatistica);
        }

        /// <summary>
        /// Exibe a página para confirmar a eliminação de uma estatística.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(m => m.EstatisticaId == id);

            if (estatistica == null)
                return NotFound();

            return View(estatistica);
        }

        /// <summary>
        /// Confirma a eliminação da estatística selecionada.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatistica = await _context.Estatistica.FindAsync(id);
            if (estatistica != null)
            {
                _context.Estatistica.Remove(estatistica);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma estatística existe na base de dados.
        /// </summary>
        private bool EstatisticaExists(int id)
        {
            return _context.Estatistica.Any(e => e.EstatisticaId == id);
        }
    }
}
