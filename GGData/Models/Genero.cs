using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Representa um género de jogo (ex: Ação, RPG, Estratégia).
    /// </summary>
    public class Genero
    {
        /// <summary>
        /// Identificador único do género.
        /// </summary>
        [Key]
        public int GeneroId { get; set; }

        /// <summary>
        /// Nome do género (máx. 50 caracteres).
        /// </summary>
        [Required, StringLength(50)]
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Relação muitos-para-muitos com jogos.
        /// </summary>
        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
