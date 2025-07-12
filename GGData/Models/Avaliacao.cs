using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GGData.Models
{
    /// <summary>
    /// Representa uma avaliação feita a um jogo por parte de um utilizador (pode ser utilizador comum ou crítico).
    /// Cada utilizador pode avaliar um jogo apenas uma vez.
    /// </summary>
    public class Avaliacao
    {
        /// <summary>
        /// Identificador único da avaliação.
        /// </summary>
        [Key]
        public int AvaliacaoId { get; set; }

        /// <summary>
        /// Nota atribuída ao jogo (entre 1 e 10).
        /// </summary>
        [Required(ErrorMessage = "A nota é obrigatória.")]
        [Range(1, 10, ErrorMessage = "A nota deve estar entre 1 e 10.")]
        public int Nota { get; set; }

        /// <summary>
        /// Comentário opcional do utilizador sobre o jogo.
        /// </summary>
        [StringLength(5000, ErrorMessage = "O comentário não pode exceder 5000 caracteres.")]
        [Display(Name = "Comentários")]
        public string? Comentarios { get; set; }

        /// <summary>
        /// Data em que a avaliação foi feita.
        /// </summary>
        [Required(ErrorMessage = "A data da review é obrigatória.")]
        [Display(Name = "Data da Review")]
        [DataType(DataType.Date)]
        public DateTime DataReview { get; set; }

        /// <summary>
        /// Tipo de utilizador que fez a avaliação: "Crítico" ou "Utilizador".
        /// Este campo é apenas informativo.
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Tipo de Utilizador")]
        [ValidateNever]
        public string? TipoUsuario { get; set; }

        // --------------------------
        // Relações (Foreign Keys)
        // --------------------------

        /// <summary>
        /// ID do utilizador que avaliou.
        /// </summary>
        [Required]
        [ForeignKey(nameof(Utilizador))]
        [Display(Name = "Utilizador")]
        public int UtilizadorId { get; set; }

        /// <summary>
        /// Objeto de navegação para o utilizador que fez a avaliação.
        /// </summary>
        [ValidateNever]
        public Utilizadores Utilizador { get; set; }

        /// <summary>
        /// ID do jogo que foi avaliado.
        /// </summary>
        [Required]
        [ForeignKey(nameof(Jogo))]
        [Display(Name = "Jogo")]
        public int JogoId { get; set; }

        /// <summary>
        /// Objeto de navegação para o jogo avaliado.
        /// </summary>
        [ValidateNever]
        public Jogo Jogo { get; set; }
    }
}
