namespace GGData.Models.ViewModels
{
    public class JogoDTO
    {
        public string Nome { get; set; }
        public string Plataforma { get; set; }
        public DateTime DataLancamento { get; set; }
        public List<string> Generos { get; set; } = new List<string>();
    }
}
