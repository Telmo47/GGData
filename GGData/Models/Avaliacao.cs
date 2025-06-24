using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Avaliação dada a cada jogo
    /// </summary>
    public class Avaliacao
    {
        /// <summary>
        /// Id da avaliação dada ao jogo
        /// </summary>
        [Key]
        public int AvaliacaoId { get; set; }

        /// <summary>
        /// Nota dada individualmente por cada usuário ao jogo
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [Range(1, 10, ErrorMessage = "A nota deve estar entre 1 e 10")]
        public int Nota { get; set; }

        /// <summary>
        /// Comentários dados pelos usuários
        /// </summary>
        [Display(Name = "Comentários")]
        public string Comentarios { get; set; }

        /// <summary>
        /// Data da review feita pelo utilizador
        /// </summary>
        [Display(Name = "Data da Review")]
        [DataType(DataType.Date)]
        public DateTime DataReview { get; set; }

        /// <summary>
        /// Tipo de usuário que deu a avaliação ao jogo, pode ser um crítico ou um usuário normal
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [StringLength(20)]
        [Display(Name = "Tipo de Usuário")]
        public string TipoUsuario { get; set; }

        // FKs

        /// <summary>
        /// Chave forasteira com referência ao utilizador que fez a avaliação
        /// </summary>
        [ForeignKey(nameof(Usuario))]
        [Display(Name = "Utilizador")]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Navegação para o utilizador que fez a avaliação
        /// </summary>
        public Usuarios Usuario { get; set; }

        /// <summary>
        /// Chave forasteira com referência ao jogo avaliado
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        [Display(Name = "Jogo")]
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo avaliado
        /// </summary>
        public Jogo Jogo { get; set; }
    }
}
