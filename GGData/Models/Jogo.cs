using System;
using System.Collections.Generic;  // necessário para List<>
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Jogos a serem notíciados e avaliados
    /// </summary>
    public class Jogo
    {
        /// <summary>
        /// Id do jogo
        /// </summary>
        [Key]
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo
        /// </summary>
        [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Genero do jogo
        /// </summary>
        [Required(ErrorMessage = "O género é obrigatório.")]
        [StringLength(50, ErrorMessage = "O género não pode ter mais de 50 caracteres.")]
        public string Genero { get; set; }

        /// <summary>
        /// Plataformas que o jogo pode ser jogado
        /// </summary>
        [Required(ErrorMessage = "A plataforma é obrigatória.")]
        [StringLength(100, ErrorMessage = "A plataforma não pode ter mais de 100 caracteres.")]
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Data de Lançamento")]
        [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Lista das avaliações associadas a este jogo
        /// </summary>
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// Lista das estatísticas associadas a este jogo
        /// </summary>
        public virtual ICollection<Estatistica> Estatisticas { get; set; } = new List<Estatistica>();
    }
}
