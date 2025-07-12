using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Representa um género de videojogo (ex: Aventura, Estratégia, RPG, etc.).
    /// </summary>
    public class Genero
    {
        /// <summary>
        /// Identificador único do género.
        /// </summary>
        [Key]
        public int GeneroId { get; set; }

        /// <summary>
        /// Nome do género (máximo de 50 caracteres).
        /// </summary>
        [Required(ErrorMessage = "O nome do género é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do género não pode exceder 50 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Relação muitos-para-muitos com jogos.
        /// Um género pode estar associado a vários jogos.
        /// </summary>
        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
