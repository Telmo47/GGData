using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GGData.Models
{
    /// <summary>
    /// Representa estatísticas agregadas de um jogo,
    /// calculadas a partir das avaliações de utilizadores e críticos.
    /// </summary>
    public class Estatistica
    {
        /// <summary>
        /// Identificador único da estatística.
        /// </summary>
        [Key]
        public int EstatisticaId { get; set; }

        /// <summary>
        /// Texto descritivo das conquistas possíveis no jogo.
        /// </summary>
        [Required(ErrorMessage = "O campo Conquistas é obrigatório.")]
        public string Conquistas { get; set; }

        /// <summary>
        /// Tempo médio que os utilizadores levam a concluir o jogo, em horas.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "O tempo médio de jogo deve ser maior ou igual a zero.")]
        [Display(Name = "Tempo Médio de Jogo (horas)")]
        public decimal TempoMedioJogo { get; set; }

        /// <summary>
        /// Número total de avaliações feitas ao jogo.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "O total de avaliações deve ser maior ou igual a zero.")]
        [Display(Name = "Total de Avaliações")]
        public int TotalAvaliacoes { get; set; }

        /// <summary>
        /// Nota média dada por utilizadores comuns, numa escala de 0 a 100.
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média das notas dos utilizadores deve estar entre 0 e 100.")]
        [Display(Name = "Média de Notas dos Utilizadores")]
        public decimal MediaNotaUtilizadores { get; set; }

        /// <summary>
        /// Nota média dada por críticos, numa escala de 0 a 100.
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média das notas dos críticos deve estar entre 0 e 100.")]
        [Display(Name = "Média de Notas dos Críticos")]
        public decimal MediaNotaCriticos { get; set; }

        /// <summary>
        /// Chave estrangeira que liga esta estatística ao jogo correspondente.
        /// </summary>
        [Required(ErrorMessage = "O jogo é obrigatório.")]
        [ForeignKey(nameof(Jogo))]
        [Display(Name = "Jogo")]
        public int JogoId { get; set; }

        /// <summary>
        /// Objeto de navegação para o jogo associado.
        /// </summary>
        [ValidateNever]
        public Jogo Jogo { get; set; }
    }
}
