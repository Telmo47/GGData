using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Representa um jogo de vídeo com suas propriedades e relações.
    /// </summary>
    public class Jogo
    {
        /// <summary>
        /// Identificador único do jogo.
        /// </summary>
        [Key]
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo (obrigatório, máximo 100 caracteres).
        /// </summary>
        [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do jogo não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        /*
         * Campo antigo "Genero" foi removido para dar lugar
         * à relação muitos-para-muitos com a entidade Genero via JogoGenero.
         */

        /// <summary>
        /// Plataforma onde o jogo está disponível (ex: PC, Xbox, PlayStation).
        /// </summary>
        [Required(ErrorMessage = "A plataforma do jogo é obrigatória.")]
        [StringLength(100, ErrorMessage = "A plataforma não pode exceder 100 caracteres.")]
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Avaliações feitas ao jogo por vários utilizadores.
        /// </summary>
        [ValidateNever]
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// FK para o utilizador associado (opcional).
        /// </summary>
        public int? UtilizadorId { get; set; }

        /// <summary>
        /// Navegação para o utilizador associado ao jogo.
        /// </summary>
        [ValidateNever]
        public Utilizadores? Utilizador { get; set; }

        /// <summary>
        /// Estatísticas agregadas deste jogo.
        /// </summary>
        [ValidateNever]
        public virtual Estatistica Estatistica { get; set; }

        /// <summary>
        /// URL para imagem de capa ou thumbnail do jogo (opcional, até 300 caracteres).
        /// </summary>
        [StringLength(300)]
        public string? ImagemUrl { get; set; }

        /// <summary>
        /// Relação muitos-para-muitos com géneros através da entidade JogoGenero.
        /// </summary>
        public ICollection<JogoGenero> JogoGeneros { get; set; } = new List<JogoGenero>();
    }
}
