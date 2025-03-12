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
        /// Tempo médio de jogo atribuido pelos utilizadores
        /// </summary>

        public decimal TempoMedioJogo { get; set; }

        /// <summary>
        /// Número total de avaliações
        /// </summary>

        public int TotalAvaliacoes { get; set; }

        //FKs


        /// <summary>
        /// Chave forasteira com referência ao Jogo
        /// </summary>
        [ForeignKey(nameof(Jogo)]
        public int JogoID { get; set; }
        public Jogo Jogo { get; set; }
    }
}
