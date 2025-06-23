using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GGData.Models
{
    /// <summary>
    /// Estatísticas agregadas de um jogo,
    /// calculadas a partir das avaliações feitas pelos utilizadores.
    /// </summary>
    public class Estatistica
    {
        /// <summary>
        /// Identificador das estatísticas.
        /// </summary>
        [Key]
        public int EstatisticaId { get; set; }

        /// <summary>
        /// Conquistas obtidas no jogo (texto).
        /// </summary>
        [Required(ErrorMessage = "O campo Conquistas é obrigatório.")]
        public string Conquistas { get; set; }

        /// <summary>
        /// Tempo médio de jogo, valor não negativo.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "O tempo médio de jogo deve ser maior ou igual a zero.")]
        public decimal TempoMedioJogo { get; set; }

        /// <summary>
        /// Total de avaliações recebidas para este jogo.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "O total de avaliações deve ser maior ou igual a zero.")]
        public int TotalAvaliacoes { get; set; }

        /// <summary>
        /// Média das notas dadas pelos utilizadores normais (0 a 100).
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média de notas dos utilizadores deve estar entre 0 e 100.")]
        public decimal MediaNotaUtilizadores { get; set; }

        /// <summary>
        /// Média das notas dadas pelos críticos (0 a 100).
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média de notas dos críticos deve estar entre 0 e 100.")]
        public decimal MediaNotaCriticos { get; set; }

        /// <summary>
        /// FK para o jogo a que estas estatísticas pertencem.
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        [Required(ErrorMessage = "O jogo é obrigatório.")]
        public int JogoId { get; set; }

        /// <summary>
        /// Navegação para o jogo.
        /// </summary>
        [ValidateNever]
        public Jogo Jogo { get; set; }
    }
}
