using GGData.Data;
using GGData.Models;
using GGData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGData.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Protegido com JWT
    public class JogosAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JogosAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JogosAuth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDTO>>> GetJogos()
        {
            var jogos = await _context.Jogos
                .Select(j => new JogoDTO
                {
                    JogoId = j.JogoId,
                    Nome = j.Nome,
                    Genero = j.Genero,
                    Plataforma = j.Plataforma,
                    DataLancamento = j.DataLancamento
                })
                .ToListAsync();

            return jogos;
        }

        // POST: api/JogosAuth
        [HttpPost]
        [Authorize(Roles = "Administrador")]  // Apenas admins podem adicionar jogos
        public async Task<ActionResult<JogoDTO>> CreateJogo(JogoDTO jogoDTO)
        {
            var jogo = new Jogo
            {
                Nome = jogoDTO.Nome,
                Genero = jogoDTO.Genero,
                Plataforma = jogoDTO.Plataforma,
                DataLancamento = jogoDTO.DataLancamento
            };

            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            // Retorna 201 Created com o jogo criado
            return CreatedAtAction(nameof(GetJogos), new { id = jogo.JogoId }, jogoDTO);
        }
    }
}
