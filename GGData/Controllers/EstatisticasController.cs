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
    /// Controlador MVC para gerir estatísticas dos jogos.
    /// Só utilizadores com o papel "Administrador" podem criar, editar ou apagar.
    /// A listagem e detalhes podem ser vistos por qualquer utilizador (AllowAnonymous).
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class EstatisticasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as estatísticas, incluindo dados do jogo.
        /// Disponível para qualquer utilizador (anónimo ou autenticado).
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var estatisticas = _context.Estatistica.Include(e => e.Jogo);
            return View(await estatisticas.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de uma estatística pelo seu ID.
        /// Disponível para qualquer utilizador.
        /// </summary>
        /// <param name="id">ID da estatística</param>
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
        /// Mostra detalhes de uma estatística por ID do jogo.
        /// Usa a mesma view que Details(int? id).
        /// Disponível para qualquer utilizador.
        /// </summary>
        /// <param name="jogoId">ID do jogo</param>
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
        /// Apenas administradores.
        /// </summary>
        public IActionResult Create()
        {
            ViewData["JogoId"] = new SelectList(_context.Jogos, "JogoId", "Nome");
            return View();
        }

        /// <summary>
        /// Recebe os dados submetidos para criar uma nova estatística.
        /// Apenas administradores.
        /// </summary>
        /// <param name="estatistica">Dados da estatística</param>
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
        /// Apenas administradores.
        /// </summary>
        /// <param name="id">ID da estatística</param>
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
        /// Recebe os dados submetidos para atualizar uma estatística.
        /// Apenas administradores.
        /// </summary>
        /// <param name="id">ID da estatística</param>
        /// <param name="estatistica">Dados atualizados</param>
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
        /// Exibe confirmação para apagar uma estatística.
        /// Apenas administradores.
        /// </summary>
        /// <param name="id">ID da estatística</param>
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
        /// Apaga uma estatística confirmada.
        /// Apenas administradores.
        /// </summary>
        /// <param name="id">ID da estatística</param>
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
        /// Verifica se uma estatística existe pelo ID.
        /// </summary>
        private bool EstatisticaExists(int id)
        {
            return _context.Estatistica.Any(e => e.EstatisticaId == id);
        }
    }
}
