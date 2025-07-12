using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Entidade de ligação para a relação muitos-para-muitos entre Jogos e Géneros.
    /// Cada instância representa a associação de um jogo a um género.
    /// </summary>
    public class JogoGenero
    {
        /// <summary>
        /// Chave estrangeira para o jogo.
        /// </summary>
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo associado.
        /// </summary>
        public Jogo Jogo { get; set; }

        /// <summary>
        /// Chave estrangeira para o género.
        /// </summary>
        public int GeneroId { get; set; }

        /// <summary>
        /// Navegação para o género associado.
        /// </summary>
        public Genero Genero { get; set; }
    }
}
