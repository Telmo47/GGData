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
    /// <summary>
    /// API para gerir avaliações de jogos.
    /// Requer autenticação via token Bearer (JWT).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AvaliacoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da aplicação (Entity Framework)</param>
        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém lista de avaliações, opcionalmente filtrada por jogo.
        /// </summary>
        /// <param name="jogoId">ID do jogo para filtrar avaliações (opcional)</param>
        /// <returns>Lista de avaliações com utilizador e jogo incluídos</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avaliacao>>> GetAvaliacoes([FromQuery] int? jogoId)
        {
            var query = _context.Avaliacao
                .Include(a => a.Utilizador)
                .Include(a => a.Jogo)
                .AsQueryable();

            if (jogoId.HasValue)
                query = query.Where(a => a.JogoId == jogoId);

            var avaliacoes = await query.ToListAsync();
            return Ok(avaliacoes);
        }

        /// <summary>
        /// Cria uma nova avaliação para um jogo pelo utilizador autenticado.
        /// </summary>
        /// <param name="avaliacao">Dados da avaliação a criar</param>
        /// <returns>Avaliação criada ou erro</returns>
        [HttpPost]
        public async Task<ActionResult<Avaliacao>> PostAvaliacao(Avaliacao avaliacao)
        {
            try
            {
                // Obter userId do token JWT (NameIdentifier ou sub)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                    return Unauthorized("Utilizador não autenticado.");

                // Obter email do utilizador a partir do token
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail == null)
                    return Unauthorized("Email do utilizador não encontrado no token.");

                // Obter o utilizador da BD pelo email
                var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario == null)
                    return Unauthorized("Utilizador não encontrado.");

                // Verificar se o utilizador já avaliou este jogo
                bool jaAvaliado = await _context.Avaliacao
                    .AnyAsync(a => a.JogoId == avaliacao.JogoId && a.UtilizadorId == usuario.Id);

                if (jaAvaliado)
                    return BadRequest("Este utilizador já avaliou este jogo.");

                // Atribuir o UtilizadorId e data atual à avaliação
                avaliacao.UtilizadorId = usuario.Id;
                avaliacao.DataReview = DateTime.UtcNow;

                // Definir TipoUsuario para evitar valores nulos na BD
                avaliacao.TipoUsuario = "Utilizador"; // Pode ser ajustado conforme a lógica do sistema

                // Adicionar avaliação à BD e guardar alterações
                _context.Avaliacao.Add(avaliacao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAvaliacoes), new { id = avaliacao.AvaliacaoId }, avaliacao);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna o status 500 com mensagem e stacktrace
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Atualiza uma avaliação existente.
        /// Só o dono da avaliação pode editar.
        /// </summary>
        /// <param name="id">ID da avaliação a atualizar</param>
        /// <param name="avaliacao">Dados atualizados da avaliação</param>
        /// <returns>Sem conteúdo se sucesso, erro se não autorizado ou inválido</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvaliacao(int id, Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
                return BadRequest();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null)
                return Unauthorized();

            // Obter avaliação existente na BD
            var avaliacaoExistente = await _context.Avaliacao.FindAsync(id);
            if (avaliacaoExistente == null)
                return NotFound();

            // Verificar se a avaliação pertence ao utilizador autenticado
            if (avaliacaoExistente.UtilizadorId != usuario.Id)
                return StatusCode(403, new { message = "Só podes editar as tu próprias avaliações." });

            // Atualizar campos modificáveis
            avaliacaoExistente.Nota = avaliacao.Nota;
            avaliacaoExistente.Comentarios = avaliacao.Comentarios;
            avaliacaoExistente.TipoUsuario = avaliacao.TipoUsuario;  // Ajustar conforme necessidade
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

        /// <summary>
        /// Apaga uma avaliação pelo ID.
        /// Só o dono da avaliação pode apagar.
        /// </summary>
        /// <param name="id">ID da avaliação a apagar</param>
        /// <returns>Sem conteúdo se sucesso, erro se não autorizado ou inexistente</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvaliacao(int id)
        {
            var avaliacao = await _context.Avaliacao.FindAsync(id);

            if (avaliacao == null)
                return NotFound();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var usuario = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null || avaliacao.UtilizadorId != usuario.Id)
                return Forbid("Só podes apagar as tuas próprias avaliações.");

            _context.Avaliacao.Remove(avaliacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
