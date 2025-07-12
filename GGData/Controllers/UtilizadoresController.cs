using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Para HttpContext.Session
using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador responsável por gerir os utilizadores do sistema.
    /// Apenas acessível a utilizadores com o papel "Administrador".
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os utilizadores.
        /// Mostra mensagem do último utilizador editado (armazenada na sessão).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var nome = HttpContext.Session.GetString("UltimoUsuarioEditadoNome");
            if (!string.IsNullOrEmpty(nome))
            {
                ViewBag.Mensagem = $"Último utilizador editado: {nome}";
            }
            return View(await _context.Utilizadores.ToListAsync());
        }

        /// <summary>
        /// Mostra detalhes de um utilizador específico.
        /// </summary>
        /// <param name="id">ID do utilizador</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null) return NotFound();

            return View(usuarios);
        }

        /// <summary>
        /// Retorna a view para criar um novo utilizador.
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" });
            return View();
        }

        /// <summary>
        /// Cria um novo utilizador no sistema.
        /// Valida se o email já existe para evitar duplicados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nome,Senha,Email,TipoUsuario")] Utilizadores usuarios)
        {
            usuarios.DataRegistro = DateTime.Now;

            if (_context.Utilizadores.Any(u => u.Email == usuarios.Email))
            {
                ModelState.AddModelError("Email", "Já existe um utilizador com este email.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        /// <summary>
        /// Retorna a view para editar um utilizador existente.
        /// Guarda dados na sessão para validação posterior.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FindAsync(id);
            if (usuarios == null) return NotFound();

            HttpContext.Session.SetInt32("UsuarioID", usuarios.Id);
            HttpContext.Session.SetString("Acao", "Usuarios/Edit");

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        /// <summary>
        /// Atualiza os dados do utilizador.
        /// Valida sessão para garantir que o processo não foi interrompido.
        /// Guarda o nome do último utilizador editado na sessão para exibir mensagem.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nome,Senha,DataRegistro,Email,TipoUsuario")] Utilizadores usuarios)
        {
            if (id != usuarios.Id) return NotFound();

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            if (usuarioIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o utilizador. Tem de reiniciar o processo.");
                ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
                return View(usuarios);
            }

            if (usuarioIDSessao != usuarios.Id || acao != "Usuarios/Edit")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.Remove("UsuarioID");
                    HttpContext.Session.Remove("Acao");

                    HttpContext.Session.SetString("UltimoUsuarioEditadoNome", usuarios.Nome);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        /// <summary>
        /// Retorna a view para confirmar a remoção de um utilizador.
        /// Guarda dados na sessão para validação posterior.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null) return NotFound();

            HttpContext.Session.SetInt32("UsuarioID", usuarios.Id);
            HttpContext.Session.SetString("Acao", "Usuarios/Delete");

            return View(usuarios);
        }

        /// <summary>
        /// Remove um utilizador da base de dados.
        /// Valida a sessão para garantir integridade do processo.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizadores = await _context.Utilizadores.FindAsync(id);

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            if (usuarioIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar o utilizador. Tem de reiniciar o processo.");
                return View(utilizadores);
            }

            if (usuarioIDSessao != id || acao != "Usuarios/Delete")
            {
                return RedirectToAction("Index");
            }

            if (utilizadores != null)
            {
                _context.Utilizadores.Remove(utilizadores);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("UsuarioID");
                HttpContext.Session.Remove("Acao");
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um utilizador existe na base de dados.
        /// </summary>
        private bool UsuariosExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
