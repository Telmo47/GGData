using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Entidade de associação para relação muitos-para-muitos entre Jogo e Genero.
    /// </summary>
    public class JogoGenero
    {
        /// <summary>
        /// Chave estrangeira para o jogo.
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo.
        /// </summary>
        public Jogo Jogo { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para o género.
        /// </summary>
        [ForeignKey(nameof(Genero))]
        public int GeneroId { get; set; }

        /// <summary>
        /// Navegação para o género.
        /// </summary>
        public Genero Genero { get; set; } = null!;
    }
}
