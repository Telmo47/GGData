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

        public int AvaliacaoId { get; set; }

        /// <summary>
        /// Nota dada individualmente por cada usuário ao jogo
        /// </summary>
        public int Nota { get; set; }

        /// <summary>
        /// Comentários dados pelos usuários
        /// </summary>

        public string Comentarios { get; set; }

        /// <summary>
        /// ???
        /// </summary>

        public DateTime DataReview { get; set; }

        /// <summary>
        /// ???
        /// </summary>

        public string TipoUsuario{ get; set; }
    }
}
