using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            // Envia a lista de tipos para a View, útil se usares DropDownListFor
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
            // Preencher automaticamente a data de registo
            usuarios.DataRegistro = DateTime.Now;

            // Verifica se o email já está em uso
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();
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
        /// <summary>
        /// Confirmação para eliminar um utilizador.
        /// </summary>
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
        /// <summary>
        /// Elimina um utilizador após confirmação.
        /// </summary>
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

        /// <summary>
        /// Verifica se um utilizador existe com o ID fornecido.
        /// </summary>
        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
