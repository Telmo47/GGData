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
    // Controlador API para gerir jogos com autenticação via JWT
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Requer token Bearer válido
    public class JogosAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injeção do contexto da base de dados
        public JogosAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JogosAuth
        // Retorna a lista de jogos com dados dos seus géneros concatenados numa string
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDTObyUser>>> GetJogos()
        {
            // Obtem todos os jogos incluindo os géneros relacionados
            var jogos = await _context.Jogos
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                // O filtro por utilizador autenticado foi removido (comentado)
                .Select(j => new JogoDTObyUser
                {
                    JogoId = j.JogoId,
                    Nome = j.Nome,
                    Plataforma = j.Plataforma,
                    DataLancamento = j.DataLancamento,
                    Genero = string.Join(", ", j.JogoGeneros.Select(jg => jg.Genero.Nome)),
                    ImagemUrl = j.ImagemUrl
                })
                .ToListAsync();

            return jogos;
        }

        // POST: api/JogosAuth
        // Apenas utilizadores com Role "Administrador" podem criar novos jogos
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<JogoDTO>> CreateJogo(JogoDTO jogoDTO)
        {
            // Obter nome do utilizador autenticado
            string? nomePessoaAutenticada = User.Identity?.Name;
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == nomePessoaAutenticada);

            if (utilizador == null)
            {
                return Unauthorized("Utilizador não encontrado.");
            }

            // Criar nova entidade Jogo
            var jogo = new Jogo
            {
                Nome = jogoDTO.Nome,
                Plataforma = jogoDTO.Plataforma,
                DataLancamento = jogoDTO.DataLancamento,
                ImagemUrl = jogoDTO.ImagemUrl,
                Utilizador = utilizador
            };

            // Adicionar géneros existentes que correspondem aos nomes enviados
            foreach (var generoNome in jogoDTO.Generos)
            {
                var genero = await _context.Set<Genero>().FirstOrDefaultAsync(g => g.Nome == generoNome);
                if (genero != null)
                {
                    jogo.JogoGeneros.Add(new JogoGenero { Genero = genero, Jogo = jogo });
                }
            }

            // Adicionar jogo à base de dados e guardar alterações
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            // Retornar 201 Created com a localização da lista de jogos
            return CreatedAtAction(nameof(GetJogos), new { id = jogo.JogoId }, jogoDTO);
        }
    }
}
