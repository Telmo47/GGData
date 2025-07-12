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
    [Authorize(Roles = "Administrador")]
    public class JogosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os jogos, com filtros opcionais por género.
        /// Permite acesso anónimo.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index(string genero)
        {
            var jogos = _context.Jogos
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatistica)
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.JogoGeneros.Any(jg => jg.Genero.Nome.ToLower().Contains(genero.ToLower())));
            }

            return View(await jogos.ToListAsync());
        }

        /// <summary>
        /// Detalhes de um jogo específico por Id.
        /// Permite acesso anónimo.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .FirstOrDefaultAsync(m => m.JogoId == id);

            if (jogo == null) return NotFound();

            return View(jogo);
        }

        /// <summary>
        /// Retorna a view para criar um novo jogo.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View();
        }

        /// <summary>
        /// Cria um novo jogo com os géneros selecionados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jogo jogo, int[] GeneroIds)
        {
            if (ModelState.IsValid)
            {
                // Inicializa os géneros associados ao jogo
                jogo.JogoGeneros = new List<JogoGenero>();

                foreach (var generoId in GeneroIds)
                {
                    jogo.JogoGeneros.Add(new JogoGenero { GeneroId = generoId });
                }

                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View(jogo);
        }

        /// <summary>
        /// Retorna a view para editar um jogo existente, carregando os géneros associados.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.JogoGeneros)
                .FirstOrDefaultAsync(j => j.JogoId == id);

            if (jogo == null) return NotFound();

            ViewBag.Generos = await _context.Generos.ToListAsync();

            // Guarda info na sessão para validação no post
            HttpContext.Session.SetInt32("JogoId", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Edit");

            return View(jogo);
        }

        /// <summary>
        /// Atualiza os dados do jogo e os géneros associados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Jogo jogo, int[] GeneroIds)
        {
            if (id != jogo.JogoId) return NotFound();

            var jogoIDSessao = HttpContext.Session.GetInt32("JogoId");
            var acao = HttpContext.Session.GetString("Acao");

            // Valida sessão para evitar edição inválida
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
                    // Atualiza propriedades do jogo
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();

                    // Atualiza géneros associados
                    var jogoAtual = await _context.Jogos
                        .Include(j => j.JogoGeneros)
                        .FirstOrDefaultAsync(j => j.JogoId == id);

                    if (jogoAtual == null) return NotFound();

                    // Limpa géneros antigos
                    jogoAtual.JogoGeneros.Clear();

                    // Adiciona géneros novos
                    foreach (var generoId in GeneroIds)
                    {
                        jogoAtual.JogoGeneros.Add(new JogoGenero { JogoId = id, GeneroId = generoId });
                    }

                    await _context.SaveChangesAsync();

                    // Remove dados da sessão
                    HttpContext.Session.Remove("JogoId");
                    HttpContext.Session.Remove("Acao");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogoExists(jogo.JogoId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Generos = await _context.Generos.ToListAsync();
            return View(jogo);
        }

        /// <summary>
        /// Retorna a view para confirmar a remoção de um jogo.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jogo = await _context.Jogos
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatistica)
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .FirstOrDefaultAsync(m => m.JogoId == id);

            if (jogo == null) return NotFound();

            // Guarda info na sessão para validação no post
            HttpContext.Session.SetInt32("JogoId", jogo.JogoId);
            HttpContext.Session.SetString("Acao", "Jogos/Delete");

            return View(jogo);
        }

        /// <summary>
        /// Elimina o jogo se não tiver avaliações ou estatísticas associadas.
        /// </summary>
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

            // Só apaga se não existirem avaliações nem estatísticas associadas
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
        /// Verifica se o jogo existe na base de dados.
        /// </summary>
        private bool JogoExists(int id)
        {
            return _context.Jogos.Any(e => e.JogoId == id);
        }
    }
}
