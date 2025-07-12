namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object para transportar dados de um jogo.
    /// </summary>
    public class JogoDTO
    {
        /// <summary>
        /// Nome do jogo.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Plataforma em que o jogo está disponível (ex: PC, PS5).
        /// </summary>
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Lista de géneros associados ao jogo.
        /// </summary>
        public List<string> Generos { get; set; } = new List<string>();

        /// <summary>
        /// URL da imagem do jogo (opcional).
        /// </summary>
        public string? ImagemUrl { get; set; }
    }
}
