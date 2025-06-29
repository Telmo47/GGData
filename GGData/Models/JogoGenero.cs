using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    public class JogoGenero
    {
        public int JogoId { get; set; }
        public Jogo Jogo { get; set; }

        public int GeneroId { get; set; }
        public Genero Genero { get; set; }
    }
}

