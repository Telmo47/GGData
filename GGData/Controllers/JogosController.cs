using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using GGData.Data;
using GGData.Models;
using System.Collections.Generic;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador para gerir CRUD dos Jogos.
    /// Apenas utilizadores com o papel "Administrador" têm permissão para usar todas as ações exceto as que têm [AllowAnonymous].
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da BD</param>
        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os jogos. Pode filtrar por género (parâmetro opcional).
        /// Acesso público (sem autenticação).
        /// </summary>
        /// <param name="genero">Nome do género para filtro (opcional)</param>
        /// <returns>Lista de jogos</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(string genero)
        {
            var jogos = _context.Jogos
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatistica)
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .AsQueryable();

            // Filtra por género caso seja especificado
            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.JogoGeneros.Any(jg => jg.Genero.Nome.ToLower().Contains(genero.ToLower())));
            }

            return View(await jogos.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de um jogo específico pelo id.
        /// Acesso público.
        /// </summary>
        /// <param name="id">Id do jogo</param>
        /// <returns>Detalhes do jogo ou NotFound se não existir</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .FirstOrDefaultAsync(m => m.JogoId == id);

            if (jogo == null)
                return NotFound();

            return View(jogo);
        }

        /// <summary>
        /// Exibe o formulário para criar um novo jogo.
        /// </summary>
        /// <returns>View com formulário</returns>
        public async Task<IActionResult> Create()
        {
            // Carrega todos os géneros para popular a lista na View
            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View();
        }

        /// <summary>
        /// Processa o formulário para criar um novo jogo com géneros associados.
        /// </summary>
        /// <param name="jogo">Dados do jogo</param>
        /// <param name="GeneroIds">Ids dos géneros selecionados</param>
        /// <returns>Redireciona para Index se sucesso, ou volta à View se erro</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jogo jogo, int[] GeneroIds)
        {
            if (ModelState.IsValid)
            {
                // Inicializa lista de géneros do jogo
                jogo.JogoGeneros = new List<JogoGenero>();

                // Associa os géneros selecionados
                foreach (var generoId in GeneroIds)
                {
                    jogo.JogoGeneros.Add(new JogoGenero { GeneroId = generoId });
                }

                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo não for válido, recarrega géneros e mostra o formulário com erros
            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View(jogo);
        }

        /// <summary>
        /// Exibe o formulário para editar um jogo existente.
        /// Guarda em sessão o id do jogo e a ação para controlo de tempo.
        /// </summary>
        /// <param name="id">Id do jogo a editar</param>
        /// <returns>View com dados do jogo ou NotFound</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.JogoGeneros)
                .FirstOrDefaultAsync(j => j.JogoId == id);

            if (jogo == null)
                return NotFound();

            ViewBag.Generos = await _context.Generos.ToListAsync();

            // Guarda na sessão para validar depois na submissão
            HttpContext.Session.SetInt32("JogoId", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Edit");

            return View(jogo);
        }

        /// <summary>
        /// Processa a edição do jogo, atualizando dados e géneros associados.
        /// Valida sessão para evitar problemas de tempo.
        /// </summary>
        /// <param name="id">Id do jogo</param>
        /// <param name="jogo">Dados atualizados do jogo</param>
        /// <param name="GeneroIds">Ids dos géneros selecionados</param>
        /// <returns>Redireciona para Index se sucesso, ou volta à View com erros</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Jogo jogo, int[] GeneroIds)
        {
            if (id != jogo.JogoId)
                return NotFound();

            // Verifica valores da sessão para controlo de tempo na edição
            var jogoIDSessao = HttpContext.Session.GetInt32("JogoId");
            var acao = HttpContext.Session.GetString("Acao");

            if (jogoIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o jogo. Tem de reiniciar o processo.");
                ViewBag.Generos = await _context.Generos.ToListAsync();
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
                    // Atualiza os dados do jogo
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();

                    // Atualiza géneros associados
                    var jogoAtual = await _context.Jogos
                        .Include(j => j.JogoGeneros)
                        .FirstOrDefaultAsync(j => j.JogoId == id);

                    if (jogoAtual == null)
                        return NotFound();

                    // Limpa géneros antigos
                    jogoAtual.JogoGeneros.Clear();

                    // Adiciona os géneros novos selecionados
                    foreach (var generoId in GeneroIds)
                    {
                        jogoAtual.JogoGeneros.Add(new JogoGenero { JogoId = id, GeneroId = generoId });
                    }

                    await _context.SaveChangesAsync();

                    // Remove dados da sessão após sucesso
                    HttpContext.Session.Remove("JogoId");
                    HttpContext.Session.Remove("Acao");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogoExists(jogo.JogoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View(jogo);
        }

        /// <summary>
        /// Exibe confirmação para apagar um jogo.
        /// Guarda id e ação na sessão para controlo.
        /// </summary>
        /// <param name="id">Id do jogo</param>
        /// <returns>View de confirmação ou NotFound</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatistica)
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .FirstOrDefaultAsync(m => m.JogoId == id);

            if (jogo == null)
                return NotFound();

            HttpContext.Session.SetInt32("JogoId", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Delete");

            return View(jogo);
        }

        /// <summary>
        /// Confirma o apagamento do jogo se não tiver avaliações nem estatísticas associadas.
        /// Valida sessão para evitar problemas.
        /// </summary>
        /// <param name="id">Id do jogo</param>
        /// <returns>Redireciona para Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogos
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatistica)
                .Include(j => j.JogoGeneros)
                .FirstOrDefaultAsync(j => j.JogoId == id);

            var jogoIDSessao = HttpContext.Session.GetInt32("JogoId");
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

            // Só apaga se não existir avaliações e estatísticas associadas
            if (jogo != null && (jogo.Avaliacoes?.Count ?? 0) == 0 && jogo.Estatistica == null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("JogoId");
                HttpContext.Session.Remove("Acao");
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um jogo existe na base de dados pelo id.
        /// </summary>
        /// <param name="id">Id do jogo</param>
        /// <returns>True se existir, false caso contrário</returns>
        private bool JogoExists(int id)
        {
            return _context.Jogos.Any(e => e.JogoId == id);
        }
    }
}
