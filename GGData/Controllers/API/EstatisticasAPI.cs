using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GGData.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EstatisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstatisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Estatisticas?jogoId=123
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

        // PUT: api/Estatisticas/5
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

        // POST: api/Estatisticas
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
