using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Para HttpContext.Session
using GGData.Data;
using GGData.Models;

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

<<<<<<< HEAD
        public async Task<IActionResult> Index(string genero)
        {
            var jogos = from j in _context.Jogo select j;
=======
        [AllowAnonymous]
        public async Task<IActionResult> Index(string genero)
        {
            var jogos = _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .AsQueryable();
>>>>>>> Autentication

            if (!string.IsNullOrEmpty(genero))
            {
                jogos = jogos.Where(j => j.Genero.Contains(genero));
            }

            return View(await jogos.ToListAsync());
        }

<<<<<<< HEAD
=======
        [AllowAnonymous]
>>>>>>> Autentication
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

            var jogo = await _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(m => m.JogoId == id);

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
<<<<<<< HEAD
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
=======
            var jogo = await _context.Jogo
                .Include(j => j.Avaliacoes)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(j => j.JogoId == id);

            if (jogo != null && jogo.Avaliacoes.Count == 0 && jogo.Estatisticas.Count == 0)
>>>>>>> Autentication
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
