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
    /// <summary>
    /// API protegida para gerir jogos associados ao utilizador autenticado.
    /// Apenas utilizadores autenticados com token Bearer podem aceder.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Protegido com JWT
    public class JogosAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto da base de dados.
        /// </summary>
        /// <param name="context">Contexto da aplicação (Entity Framework)</param>
        public JogosAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de jogos associados ao utilizador autenticado.
        /// </summary>
        /// <returns>Lista de jogos com os respetivos géneros concatenados</returns>
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

        /// <summary>
        /// Cria um novo jogo e associa ao utilizador autenticado.
        /// Apenas utilizadores com o papel "Administrador" podem executar esta ação.
        /// </summary>
        /// <param name="jogoDTO">Dados do jogo a criar</param>
        /// <returns>Jogo criado ou erro</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador")]  // Apenas admins podem adicionar jogos
        public async Task<ActionResult<JogoDTO>> CreateJogo(JogoDTO jogoDTO)
        {
            string? nomePessoaAutenticada = User.Identity?.Name;

            // Obter o utilizador autenticado na base de dados
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == nomePessoaAutenticada);

            if (utilizador == null)
            {
                return Unauthorized("Utilizador não encontrado.");
            }

            // Criar o objeto Jogo e associar ao utilizador
            var jogo = new Jogo
            {
                Nome = jogoDTO.Nome,
                Plataforma = jogoDTO.Plataforma,
                DataLancamento = jogoDTO.DataLancamento,
                Utilizador = utilizador
            };

            // Adicionar os géneros associados ao jogo (assume-se que já existem na BD)
            foreach (var generoNome in jogoDTO.Generos)
            {
                var genero = await _context.Set<Genero>().FirstOrDefaultAsync(g => g.Nome == generoNome);
                if (genero != null)
                {
                    jogo.JogoGeneros.Add(new JogoGenero { Genero = genero, Jogo = jogo });
                }
            }

            // Adicionar o jogo ao contexto e guardar na base de dados
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJogos), new { id = jogo.JogoId }, jogoDTO);
        }
    }
}
