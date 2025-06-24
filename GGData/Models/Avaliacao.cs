using System;
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
        /// Nota dada pelo utilizador, de 1 a 10.
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [Range(1, 10, ErrorMessage = "A nota deve estar entre 1 e 10")]
        public int Nota { get; set; }

        /// <summary>
        /// Comentários opcionais sobre o jogo.
        /// Pode ser uma mensagem livre, até 5000 caracteres.
        /// </summary>
        [StringLength(5000, ErrorMessage = "O comentário não pode exceder 5000 caracteres.")]
        [Display(Name = "Comentários")]
        public string? Comentarios { get; set; }

        /// <summary>
        /// Data da review feita pelo utilizador.
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [Display(Name = "Data da Review")]
        [DataType(DataType.Date)]
        public DateTime DataReview { get; set; }

        /// <summary>
        /// Tipo de usuário que deu a avaliação (Crítico ou Utilizador).
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [StringLength(20)]
        [Display(Name = "Tipo de Usuário")]
        public string TipoUsuario { get; set; }

        // Foreign Keys e Navegações

        /// <summary>
        /// Chave estrangeira com referência ao utilizador que fez a avaliação.
        /// </summary>
        [ForeignKey(nameof(Usuario))]
        [Display(Name = "Utilizador")]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Navegação para o utilizador que fez a avaliação.
        /// </summary>
        [ValidateNever]
        public Usuarios Usuario { get; set; }

        /// <summary>
        /// Chave estrangeira com referência ao jogo avaliado.
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        [Display(Name = "Jogo")]
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo avaliado.
        /// </summary>
        [ValidateNever]
        public Jogo Jogo { get; set; }
    }
}
