using System;
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
    /// </summary>
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
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarios == null) return NotFound();

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" });
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nome,Senha,Email,TipoUsuario")] Usuarios usuarios)
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

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null) return NotFound();

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nome,Senha,DataRegistro,Email,TipoUsuario")] Usuarios usuarios)
        {
            if (id != usuarios.UsuarioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.UsuarioId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarios == null) return NotFound();

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
    }
}
