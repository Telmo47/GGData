using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GGData.Controllers.API
{
    /// <summary>
    /// API para gerir estatísticas relacionadas com jogos.
    /// Requer autenticação via token Bearer (JWT).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EstatisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da aplicação (Entity Framework)</param>
        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a estatística associada a um jogo específico.
        /// </summary>
        /// <param name="jogoId">ID do jogo para obter a estatística</param>
        /// <returns>Estatística do jogo ou NotFound se não existir</returns>
        [HttpGet]
        public async Task<ActionResult<Estatistica>> GetEstatistica([FromQuery] int jogoId)
        {
            var estatistica = await _context.Estatistica
                .Include(e => e.Jogo)
                .FirstOrDefaultAsync(e => e.JogoId == jogoId);

            if (estatistica == null)
                return NotFound();

            return Ok(estatistica);
        }

        /// <summary>
        /// Atualiza uma estatística existente.
        /// </summary>
        /// <param name="id">ID da estatística a atualizar</param>
        /// <param name="estatistica">Dados atualizados da estatística</param>
        /// <returns>Sem conteúdo se sucesso, BadRequest se id inválido, NotFound se não existir</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstatistica(int id, Estatistica estatistica)
        {
            if (id != estatistica.EstatisticaId)
                return BadRequest();

            _context.Entry(estatistica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Estatistica.Any(e => e.EstatisticaId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Cria uma nova estatística para um jogo.
        /// </summary>
        /// <param name="estatistica">Dados da nova estatística</param>
        /// <returns>Estatística criada ou BadRequest se já existir para o jogo</returns>
        [HttpPost]
        public async Task<ActionResult<Estatistica>> PostEstatistica(Estatistica estatistica)
        {
            // Verifica se já existe estatística para o jogo
            var existe = await _context.Estatistica.AnyAsync(e => e.JogoId == estatistica.JogoId);
            if (existe)
                return BadRequest("Já existe uma estatística para este jogo.");

            _context.Estatistica.Add(estatistica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEstatistica), new { jogoId = estatistica.JogoId }, estatistica);
        }
    }
}
