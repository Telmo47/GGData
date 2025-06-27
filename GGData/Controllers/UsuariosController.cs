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
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        // GET: Usuarios
=======
>>>>>>> Autentication
        public async Task<IActionResult> Index()
        {
            var nome = HttpContext.Session.GetString("UltimoUsuarioEditadoNome");
            if (!string.IsNullOrEmpty(nome))
            {
                ViewBag.Mensagem = $"Último utilizador editado: {nome}";
            }
            return View(await _context.Usuarios.ToListAsync());
        }

<<<<<<< HEAD
        // GET: Usuarios/Details/5
=======
>>>>>>> Autentication
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarios == null) return NotFound();

            return View(usuarios);
        }

<<<<<<< HEAD
        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" });
            return View();
        }

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
=======
>>>>>>> Autentication
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null) return NotFound();


            // Guardar dados para proteção
            HttpContext.Session.SetInt32("UsuarioID", usuarios.UsuarioId);
            HttpContext.Session.SetString("Acao", "Usuarios/Edit");

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.tipoUsuario);

            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
=======

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

>>>>>>> Autentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nome,Senha,DataRegistro,Email,TipoUsuario")] Usuarios usuarios)
        {
            if (id != usuarios.UsuarioId) return NotFound();

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            if (usuarioIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue alterar o utilizador. Tem de reiniciar o processo.");
                ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
                return View(usuarios);
            }

            if (usuarioIDSessao != usuarios.UsuarioId || acao != "Usuarios/Edit")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();

                    // Limpar sessão após sucesso
                    HttpContext.Session.Remove("UsuarioID");
                    HttpContext.Session.Remove("Acao");
                }
                catch (DbUpdateConcurrencyException)
                {
<<<<<<< HEAD
                    if (!_context.Usuarios.Any(e => e.UsuarioId == id)) return NotFound();
=======
                    if (!UsuariosExists(usuarios.UsuarioId))
                        return NotFound();
>>>>>>> Autentication
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tipos = new SelectList(new[] { "Critico", "Utilizador" }, usuarios.TipoUsuario);
            return View(usuarios);
        }

<<<<<<< HEAD
        // GET: Usuarios/Delete/5
=======
>>>>>>> Autentication
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuarios == null) return NotFound();


            // Guardar dados para proteção
            HttpContext.Session.SetInt32("UsuarioID", usuarios.UsuarioId);
            HttpContext.Session.SetString("Acao", "Usuarios/Delete");

            return View(usuarios);
        }

<<<<<<< HEAD
        // POST: Usuarios/Delete/5
=======
>>>>>>> Autentication
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);

            var usuarioIDSessao = HttpContext.Session.GetInt32("UsuarioID");
            var acao = HttpContext.Session.GetString("Acao");

            if (usuarioIDSessao == null || string.IsNullOrEmpty(acao))
            {
                ModelState.AddModelError("", "Demorou muito tempo. Já não consegue eliminar o utilizador. Tem de reiniciar o processo.");
                return View(usuarios);
            }

            if (usuarioIDSessao != id || acao != "Usuarios/Delete")
            {
                return RedirectToAction("Index");
            }

            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("UsuarioID");
                HttpContext.Session.Remove("Acao");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}

