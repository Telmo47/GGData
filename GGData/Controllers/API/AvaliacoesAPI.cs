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
    // Controlador API para gerir avaliações, com autenticação via token JWT
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Requer token Bearer para acesso
    public class AvaliacoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injeção do contexto da base de dados
        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Avaliacoes?jogoId=123
        // Obtém lista de avaliações, opcionalmente filtradas por jogo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avaliacao>>> GetAvaliacoes([FromQuery] int? jogoId)
        {
            var query = _context.Avaliacao
                .Include(a => a.Utilizador) // Incluir dados do utilizador
                .Include(a => a.Jogo)       // Incluir dados do jogo
                .AsQueryable();

            if (jogoId.HasValue)
                query = query.Where(a => a.JogoId == jogoId);

            var avaliacoes = await query.ToListAsync();
            return Ok(avaliacoes);
        }

        // POST: api/Avaliacoes
        // Cria uma nova avaliação associada ao utilizador autenticado
        [HttpPost]
        public async Task<ActionResult<Avaliacao>> PostAvaliacao(Avaliacao avaliacao)
        {
            try
            {
                // Extrai o userId do token JWT (NameIdentifier ou sub)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                    return Unauthorized("Utilizador não autenticado.");

                // Extrai o email do utilizador do token
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail == null)
                    return Unauthorized("Email do utilizador não encontrado no token.");

                // Procura o utilizador na BD pelo email
                var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario == null)
                    return Unauthorized("Utilizador não encontrado.");

                // Verifica se este utilizador já avaliou este jogo
                bool jaAvaliado = await _context.Avaliacao
                    .AnyAsync(a => a.JogoId == avaliacao.JogoId && a.UtilizadorId == usuario.Id);

                if (jaAvaliado)
                    return BadRequest("Este utilizador já avaliou este jogo.");

                // Define o Id do utilizador e data da avaliação
                avaliacao.UtilizadorId = usuario.Id;
                avaliacao.DataReview = DateTime.UtcNow;

                // Define TipoUsuario para evitar NULL na BD (exemplo fixo, pode ser ajustado)
                avaliacao.TipoUsuario = "Utilizador"; // ou "Critico", conforme lógica da app

                // Adiciona avaliação e guarda na base de dados
                _context.Avaliacao.Add(avaliacao);
                await _context.SaveChangesAsync();

                // Retorna a avaliação criada com código 201 Created
                return CreatedAtAction(nameof(GetAvaliacoes), new { id = avaliacao.AvaliacaoId }, avaliacao);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna erro 500 com detalhes
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // PUT: api/Avaliacoes/5
        // Atualiza uma avaliação existente (somente pelo dono)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvaliacao(int id, Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
                return BadRequest();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null)
                return Unauthorized();

            // Busca a avaliação existente na BD
            var avaliacaoExistente = await _context.Avaliacao.FindAsync(id);
            if (avaliacaoExistente == null)
                return NotFound();

            // Só permite editar se for o dono da avaliação
            if (avaliacaoExistente.UtilizadorId != usuario.Id)
                return StatusCode(403, new { message = "Só podes editar as tu próprias avaliações." });

            // Atualiza campos permitidos
            avaliacaoExistente.Nota = avaliacao.Nota;
            avaliacaoExistente.Comentarios = avaliacao.Comentarios;
            avaliacaoExistente.TipoUsuario = avaliacao.TipoUsuario;  // ou manter existente, conforme lógica
            avaliacaoExistente.DataReview = DateTime.UtcNow;         // Atualiza data da revisão

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verifica se a avaliação ainda existe em caso de concorrência
                if (!_context.Avaliacao.Any(e => e.AvaliacaoId == id))
                    return NotFound();

                throw;
            }

            // Retorna 204 No Content em sucesso
            return NoContent();
        }

        // DELETE: api/Avaliacoes/5
        // Apaga avaliação (somente pelo dono)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvaliacao(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao == null)
                return NotFound();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);

            // Só permite apagar se for o dono da avaliação
            if (usuario == null || avaliacao.UtilizadorId != usuario.Id)
                return Forbid("Só podes apagar as tuas próprias avaliações.");

            // Remove a avaliação e guarda mudanças
            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();

            // Retorna 204 No Content em sucesso
            return NoContent();
        }
    }
}
