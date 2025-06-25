namespace GGData.Models.ViewModels
{
    public class JogoDTObyUser
    {
        public int JogoId { get; set; }
        public string Nome { get; set; }
        public string Genero { get; set; }
        public string Plataforma { get; set; }
        public DateTime DataLancamento { get; set; }

        // Podes adicionar outros campos que façam sentido para o utilizador
    }
}
