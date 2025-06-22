using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Estatísticas de cada jogo
    /// </summary>
    public class Estatistica
    {
        /// <summary>
        /// Id das estatísticas do jogo
        /// </summary>
        [Key]
        public int EstatisticaId { get; set; }

        /// <summary>
        /// Conquistas obtidas
        /// </summary>
        [Required(ErrorMessage = "O campo Conquistas é obrigatório.")]
        public string Conquistas { get; set; }

        /// <summary>
        /// Tempo médio de jogo atribuído pelos utilizadores
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "O tempo médio de jogo deve ser maior ou igual a zero.")]
        public decimal TempoMedioJogo { get; set; }

        /// <summary>
        /// Número total de avaliações
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "O total de avaliações deve ser maior ou igual a zero.")]
        public int TotalAvaliacoes { get; set; }

        /// <summary>
        /// Média das notas dadas pelos utilizadores comuns
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média de notas dos utilizadores deve estar entre 0 e 100.")]
        public decimal MediaNotaUtilizadores { get; set; }

        /// <summary>
        /// Média das notas dadas pelos críticos
        /// </summary>
        [Range(0, 100, ErrorMessage = "A média de notas dos críticos deve estar entre 0 e 100.")]
        public decimal MediaNotaCriticos { get; set; }

        // FK

        /// <summary>
        /// Chave forasteira com referência ao Jogo
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        [Required(ErrorMessage = "O jogo é obrigatório.")]
        public int JogoID { get; set; }

        public Jogo Jogo { get; set; }
    }
}
