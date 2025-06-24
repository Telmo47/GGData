using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador responsável por gerir os utilizadores do sistema.
    /// Inclui operações CRUD: listar, editar, ver detalhes e eliminar.
    /// A criação de utilizadores é feita pelo Identity.
    /// </summary>
    [Authorize] // Apenas utilizadores autenticados podem aceder
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuarios == null)
                return NotFound();

            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
                return NotFound();

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
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
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
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
