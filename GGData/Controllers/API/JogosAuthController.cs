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
        public async Task<ActionResult<IEnumerable<JogoDTObyUser>>> GetJogos()
        {
            string? nomePessoaAutenticada = User.Identity?.Name;

            var jogos = await _context.Jogos
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .Where(j => j.Utilizador != null && j.Utilizador.UserName == nomePessoaAutenticada)
                .Select(j => new JogoDTObyUser
                {
                    JogoId = j.JogoId,
                    Nome = j.Nome,
                    Plataforma = j.Plataforma,
                    DataLancamento = j.DataLancamento,
                    Genero = string.Join(", ", j.JogoGeneros.Select(jg => jg.Genero.Nome))
                })
                .ToListAsync();

            return jogos;
        }

        // POST: api/JogosAuth
        [HttpPost]
        [Authorize(Roles = "Administrador")]  // Apenas admins podem adicionar jogos
        public async Task<ActionResult<JogoDTO>> CreateJogo(JogoDTO jogoDTO)
        {
            string? nomePessoaAutenticada = User.Identity?.Name;
            var utilizador = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == nomePessoaAutenticada);

            if (utilizador == null)
            {
                return Unauthorized("Utilizador não encontrado.");
            }

            var jogo = new Jogo
            {
                Nome = jogoDTO.Nome,
                Plataforma = jogoDTO.Plataforma,
                DataLancamento = jogoDTO.DataLancamento,
                Utilizador = utilizador
            };

            // Adiciona os géneros (assumindo que já existem no BD)
            foreach (var generoNome in jogoDTO.Generos)
            {
                var genero = await _context.Set<Genero>().FirstOrDefaultAsync(g => g.Nome == generoNome);
                if (genero != null)
                {
                    jogo.JogoGeneros.Add(new JogoGenero { Genero = genero, Jogo = jogo });
                }
            }

            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJogos), new { id = jogo.JogoId }, jogoDTO);
        }
    }
}
