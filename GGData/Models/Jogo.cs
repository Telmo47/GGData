using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    public class Jogo
    {
        [Key]
        public int JogoId { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; }

        // Removido o campo Genero string para substituir pela relação N-M
        // [Required, StringLength(50)]
        // public string Genero { get; set; }

        [Required, StringLength(100)]
        public string Plataforma { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DataLancamento { get; set; }

        [ValidateNever]
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        public int? UtilizadorId { get; set; }  // FK para Usuarios

        [ValidateNever]
        public Usuarios? Utilizador { get; set; }  // Navegação

        [ValidateNever]
        public virtual Estatistica Estatistica { get; set; }

        [StringLength(300)]
        public string? ImagemUrl { get; set; }

        // Relação N-M com Genero
        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
