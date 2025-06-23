using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Necessário para HttpContext.Session
using GGData.Data;
using GGData.Models;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador responsável por gerir os utilizadores do sistema.
    /// Inclui operações CRUD: listar, criar, editar, ver detalhes e eliminar.
    /// </summary>
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        /// <summary>
        /// Lista todos os utilizadores registados.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Verificar se existe informação de sessão para o último utilizador editado
            var nome = HttpContext.Session.GetString("UltimoUsuarioEditadoNome");
            if (!string.IsNullOrEmpty(nome))
            {
                ViewBag.Mensagem = $"Último utilizador editado: {nome}";
            }

            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        /// <summary>
        /// Mostra os detalhes de um utilizador específico.
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuarios == null)
                return NotFound();

            return View(usuarios);
        }

        // GET: Usuarios/Create
        /// <summary>
        /// Apresenta o formulário para criar um novo utilizador.
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" });
            return View();
        }

        // POST: Usuarios/Create
        /// <summary>
        /// Processa a criação de um novo utilizador.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nome,Senha,Email,tipoUsuario")] Usuarios usuarios)
        {
            usuarios.DataRegistro = DateTime.Now;

            if (_context.Usuarios.Any(u => u.Email == usuarios.Email))
            {
                ModelState.AddModelError("Email", "Já existe um utilizador com este email.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.tipoUsuario);
            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        /// <summary>
        /// Apresenta o formulário para editar um utilizador existente.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
                return NotFound();

            // Guardar os dados do último utilizador editado na sessão
            HttpContext.Session.SetInt32("UltimoUsuarioEditadoID", usuarios.UsuarioId);
            HttpContext.Session.SetString("UltimoUsuarioEditadoNome", usuarios.Nome);

            // Guardar ID para proteção contra adulteração (igual ao passo 3 do professor)
            HttpContext.Session.SetInt32("UsuarioID", usuarios.UsuarioId);

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.tipoUsuario);
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
        /// <summary>
        /// Processa a edição dos dados do utilizador.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nome,Senha,DataRegistro,Email,tipoUsuario")] Usuarios usuarios)
        {
            if (id != usuarios.UsuarioId)
                return NotFound();

            // Ler o ID guardado na sessão para comparar
            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");

            if (usuarioIDSessao == null)
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o utilizador. Tem de reiniciar o processo.");
                ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.tipoUsuario);
                return View(usuarios);
            }

            if (usuarioIDSessao != usuarios.UsuarioId)
            {
                // Tentativa de adulteração
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();

                    // Limpar ID da sessão após sucesso
                    HttpContext.Session.Remove("UsuarioID");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.UsuarioId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.tipoUsuario);
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuarios == null)
                return NotFound();

            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
