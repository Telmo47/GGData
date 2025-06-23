using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GGData.Models
{
    /// <summary>
    /// Avaliação dada a cada jogo por um utilizador.
    /// Cada utilizador pode avaliar um jogo uma única vez.
    /// </summary>
    public class Avaliacao
    {
        /// <summary>
        /// Id da avaliação.
        /// </summary>
        [Key]
        public int AvaliacaoId { get; set; }

        /// <summary>
        /// Nota dada pelo utilizador, de 0 a 100.
        /// </summary>
        [Range(0, 100, ErrorMessage = "A nota deve estar entre 0 e 100.")]
        public int Nota { get; set; }

        /// <summary>
        /// Comentários opcionais sobre o jogo.
        /// Pode ser uma mensagem livre, até 5000 caracteres.
        /// </summary>
        [StringLength(5000, ErrorMessage = "O comentário não pode exceder 5000 caracteres.")]
        public string? Comentarios { get; set; }

        /// <summary>
        /// Data em que a avaliação foi feita.
        /// </summary>
        public DateTime DataReview { get; set; }

        /// <summary>
        /// Tipo de usuário que deu a avaliação (Crítico ou Utilizador).
        /// </summary>
        [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
        [StringLength(20)]
        [Display(Name = "Tipo de Usuário")]
        public string TipoUsuario { get; set; }

        // FKs e navegações

        /// <summary>
        /// FK para o utilizador que fez a avaliação.
        /// </summary>
        [ForeignKey(nameof(Usuario))]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Navegação para o utilizador.
        /// </summary>
        [ValidateNever]
        public Usuarios Usuario { get; set; }

        /// <summary>
        /// FK para o jogo avaliado.
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo.
        /// </summary>
        [ValidateNever]
        public Jogo Jogo { get; set; }
    }
}
