using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GGData.Controllers.API
{
    // Controlador API para gerir estatísticas relacionadas com jogos
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Requer autenticação JWT Bearer
    public class EstatisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injeção do contexto da base de dados
        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Estatisticas?jogoId=123
        // Se for passado jogoId, retorna estatística específica para esse jogo
        // Caso contrário, retorna lista de todas as estatísticas com dados do jogo incluídos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estatistica>>> GetEstatisticas([FromQuery] int? jogoId)
        {
            if (jogoId.HasValue)
            {
                // Obter estatística do jogo específico com Include para dados do jogo
                var estatistica = await _context.Estatistica
                    .Include(e => e.Jogo)
                    .FirstOrDefaultAsync(e => e.JogoId == jogoId.Value);

                if (estatistica == null)
                    return NotFound();

                return Ok(estatistica);
            }
            else
            {
                // Obter todas as estatísticas, incluindo dados dos jogos relacionados
                var estatisticas = await _context.Estatistica
                    .Include(e => e.Jogo)
                    .ToListAsync();

                return Ok(estatisticas);
            }
        }

        // PUT: api/Estatisticas/5
        // Atualiza uma estatística existente com base no id fornecido
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstatistica(int id, Estatistica estatistica)
        {
            // Verifica se o id na rota corresponde ao id do objeto enviado
            if (id != estatistica.EstatisticaId)
                return BadRequest();

            _context.Entry(estatistica).State = EntityState.Modified;

            try
            {
                // Tenta guardar alterações
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se ocorrer concorrência, verifica se a estatística ainda existe
                if (!_context.Estatistica.Any(e => e.EstatisticaId == id))
                    return NotFound();

                throw;
            }

            // Retorna 204 No Content em caso de sucesso
            return NoContent();
        }

        // POST: api/Estatisticas
        // Cria uma nova estatística para um jogo
        [HttpPost]
        public async Task<ActionResult<Estatistica>> PostEstatistica(Estatistica estatistica)
        {
            // Verifica se já existe estatística para o jogo para evitar duplicados
            var existe = await _context.Estatistica.AnyAsync(e => e.JogoId == estatistica.JogoId);
            if (existe)
                return BadRequest("Já existe uma estatística para este jogo.");

            // Adiciona nova estatística e guarda na BD
            _context.Estatistica.Add(estatistica);
            await _context.SaveChangesAsync();

            // Retorna 201 Created com localização da nova estatística
            return CreatedAtAction(nameof(GetEstatisticas), new { jogoId = estatistica.JogoId }, estatistica);
        }
    }
}
