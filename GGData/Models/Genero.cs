using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    public class Genero
    {
        [Key]
        public int GeneroId { get; set; }

        [Required, StringLength(50)]
        public string Nome { get; set; }

        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
