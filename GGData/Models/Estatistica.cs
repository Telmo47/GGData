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

        public int EstatisticaId { get; set; }

        /// <summary>
        /// Tempo médio de jogo atribuido pelos utilizadores
        /// </summary>

        public decimal TempoMedioJogo { get; set; }

        /// <summary>
        /// Número total de avaliações
        /// </summary>

        public int TotalAvaliacoes { get; set; }
    }
}
