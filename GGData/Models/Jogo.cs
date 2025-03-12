using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{

    /// <summary>
    /// Jogos a serem notíciados e avaliados
    /// </summary>
    public class Jogo
    {

        /// <summary>
        /// Id do jogo
        /// </summary>
        [Key]
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Genero do jogo
        /// </summary>
        public string Genero { get; set; }

        /// <summary>
        /// Plataformas que o jogo pode ser jogado
        /// </summary>
        public string Plataforma { get; set; }


    }
}
