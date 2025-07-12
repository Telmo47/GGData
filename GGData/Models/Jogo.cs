using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Representa um jogo na plataforma GGData.
    /// </summary>
    public class Jogo
    {
        /// <summary>
        /// Identificador único do jogo.
        /// </summary>
        [Key]
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo (máximo 100 caracteres).
        /// </summary>
        [Required, StringLength(100)]
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Plataforma onde o jogo está disponível (máximo 100 caracteres).
        /// </summary>
        [Required, StringLength(100)]
        public string Plataforma { get; set; } = null!;

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        [Required, DataType(DataType.Date)]
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Coleção de avaliações feitas para este jogo.
        /// </summary>
        [ValidateNever]
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// Foreign key para o utilizador que adicionou o jogo (opcional).
        /// </summary>
        public int? UtilizadorId { get; set; }

        /// <summary>
        /// Navegação para o utilizador que adicionou o jogo (opcional).
        /// </summary>
        [ValidateNever]
        public Utilizadores? Utilizador { get; set; }

        /// <summary>
        /// Estatísticas associadas a este jogo.
        /// </summary>
        [ValidateNever]
        public virtual Estatistica Estatistica { get; set; } = null!;

        /// <summary>
        /// URL da imagem representativa do jogo (opcional, máximo 300 caracteres).
        /// </summary>
        [StringLength(300)]
        public string? ImagemUrl { get; set; }

        /// <summary>
        /// Relação muitos-para-muitos com géneros do jogo.
        /// </summary>
        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
