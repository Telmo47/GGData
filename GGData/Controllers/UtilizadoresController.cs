using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Para usar HttpContext.Session
using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador para gerir os utilizadores do sistema.
    /// Acesso restrito a utilizadores com papel de Administrador.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public UtilizadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os utilizadores.
        /// Mostra mensagem com o nome do último utilizador editado (guardado em sessão).
        /// </summary>
        /// <returns>View com lista de utilizadores</returns>
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
        /// Mostra detalhes de um utilizador pelo id.
        /// </summary>
        /// <param name="id">Id do utilizador</param>
        /// <returns>View com detalhes ou NotFound</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null) return NotFound();

            return View(usuarios);
        }

        /// <summary>
        /// Mostra formulário para criar novo utilizador.
        /// </summary>
        /// <returns>View com formulário</returns>
        public IActionResult Create()
        {
            // Define tipos de utilizadores possíveis para dropdown
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" });
            return View();
        }

        /// <summary>
        /// Processa a criação de um novo utilizador.
        /// Verifica se o email já existe antes de criar.
        /// </summary>
        /// <param name="usuarios">Dados do novo utilizador</param>
        /// <returns>Redireciona para lista se sucesso, ou volta ao formulário com erros</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nome,Senha,Email,TipoUsuario")] Utilizadores usuarios)
        {
            usuarios.DataRegistro = DateTime.Now;

            // Valida se já existe utilizador com o email fornecido
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
        /// Mostra formulário para editar um utilizador existente.
        /// Guarda dados na sessão para controlo do tempo de edição.
        /// </summary>
        /// <param name="id">Id do utilizador a editar</param>
        /// <returns>View com dados do utilizador ou NotFound</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FindAsync(id);
            if (usuarios == null) return NotFound();

            // Guarda na sessão id e ação para controlo da edição
            HttpContext.Session.SetInt32("UsuarioID", usuarios.Id);
            HttpContext.Session.SetString("Acao", "Usuarios/Edit");

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        /// <summary>
        /// Processa a edição dos dados do utilizador.
        /// Valida sessão para garantir integridade.
        /// </summary>
        /// <param name="id">Id do utilizador</param>
        /// <param name="usuarios">Dados atualizados</param>
        /// <returns>Redireciona para lista se sucesso ou volta ao formulário com erros</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nome,Senha,DataRegistro,Email,TipoUsuario")] Utilizadores usuarios)
        {
            if (id != usuarios.Id) return NotFound();

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            // Verifica sessão válida para edição
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

                    // Limpa sessão após sucesso na edição
                    HttpContext.Session.Remove("UsuarioID");
                    HttpContext.Session.Remove("Acao");

                    // Guarda o nome do último utilizador editado para mensagem na lista
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
        /// Mostra confirmação para eliminar um utilizador.
        /// Guarda dados na sessão para controlo do processo.
        /// </summary>
        /// <param name="id">Id do utilizador</param>
        /// <returns>View para confirmação ou NotFound</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null) return NotFound();

            // Guarda na sessão para controlo do processo de eliminação
            HttpContext.Session.SetInt32("UsuarioID", usuarios.Id);
            HttpContext.Session.SetString("Acao", "Usuarios/Delete");

            return View(usuarios);
        }

        /// <summary>
        /// Processa a confirmação da eliminação do utilizador.
        /// Valida sessão para evitar problemas de tempo.
        /// </summary>
        /// <param name="id">Id do utilizador a eliminar</param>
        /// <returns>Redireciona para a lista após eliminação</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizadores = await _context.Utilizadores.FindAsync(id);

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            // Valida sessão antes de apagar
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

                // Limpa sessão após eliminação
                HttpContext.Session.Remove("UsuarioID");
                HttpContext.Session.Remove("Acao");
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um utilizador existe pelo id.
        /// </summary>
        /// <param name="id">Id do utilizador</param>
        /// <returns>True se existir, false caso contrário</returns>
        private bool UsuariosExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
