using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace GGData.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Avaliacoes?jogoId=123
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avaliacao>>> GetAvaliacoes([FromQuery] int? jogoId)
        {
            var query = _context.Avaliacao
                .Include(a => a.Usuario)
                .Include(a => a.Jogo)
                .AsQueryable();

            if (jogoId.HasValue)
                query = query.Where(a => a.JogoId == jogoId);

            var avaliacoes = await query.ToListAsync();
            return Ok(avaliacoes);
        }

        // POST: api/Avaliacoes
        [HttpPost]
        public async Task<ActionResult<Avaliacao>> PostAvaliacao(Avaliacao avaliacao)
        {
            try
            {
                // Obter userId do token JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                    return Unauthorized("Utilizador não autenticado.");

                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail == null)
                    return Unauthorized("Email do utilizador não encontrado no token.");

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario == null)
                    return Unauthorized("Utilizador não encontrado.");

                // Verificar se já existe avaliação para este usuário e jogo
                bool jaAvaliado = await _context.Avaliacao
                    .AnyAsync(a => a.JogoId == avaliacao.JogoId && a.UsuarioId == usuario.UsuarioId);

                if (jaAvaliado)
                    return BadRequest("Este utilizador já avaliou este jogo.");

                // Atribuir o UsuarioId ao avaliacao
                avaliacao.UsuarioId = usuario.UsuarioId;
                avaliacao.DataReview = DateTime.UtcNow;

                // Definir TipoUsuario para evitar erro de NULL na BD
                avaliacao.TipoUsuario = "Utilizador"; // ou "Critico", conforme a tua lógica

                _context.Avaliacao.Add(avaliacao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAvaliacoes), new { id = avaliacao.AvaliacaoId }, avaliacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // PUT: api/Avaliacoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvaliacao(int id, Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
                return BadRequest();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null)
                return Unauthorized();

            // Buscar avaliação existente na BD
            var avaliacaoExistente = await _context.Avaliacao.FindAsync(id);
            if (avaliacaoExistente == null)
                return NotFound();

            // Verificar se o dono da avaliação é o utilizador autenticado
            if (avaliacaoExistente.UsuarioId != usuario.UsuarioId)
                return StatusCode(403, new { message = "Só podes editar as tu próprias avaliações." });

            // Atualizar os campos que podem ser modificados
            avaliacaoExistente.Nota = avaliacao.Nota;
            avaliacaoExistente.Comentarios = avaliacao.Comentarios;
            avaliacaoExistente.TipoUsuario = avaliacao.TipoUsuario;  // ou manter o existente, conforme lógica
            avaliacaoExistente.DataReview = DateTime.UtcNow; // Atualiza a data da revisão

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Avaliacao.Any(e => e.AvaliacaoId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }



        // DELETE: api/Avaliacoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvaliacao(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao == null)
                return NotFound();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null || avaliacao.UsuarioId != usuario.UsuarioId)
                return Forbid("Só podes apagar as tuas próprias avaliações.");

            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
