using System;

namespace GGData.Models.ViewModels
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para representar jogos associados a um utilizador específico.
    /// </summary>
    public class JogoDTObyUser
    {
        /// <summary>
        /// Identificador único do jogo.
        /// </summary>
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Plataforma onde o jogo está disponível (ex: PC, PS5, Xbox).
        /// </summary>
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Géneros do jogo concatenados numa única string (ex: "Ação, Aventura").
        /// Usado principalmente para leitura (GET).
        /// </summary>
        public string Genero { get; set; }

        // Nota: Para envio de dados (POST), usa-se outro DTO, como JogoDTO.
    }
}
