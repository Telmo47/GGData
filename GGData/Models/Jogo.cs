﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Representa um jogo que pode ser avaliado no sistema.
    /// </summary>
    public class Jogo
    {
        /// <summary>
        /// Id único do jogo.
        /// </summary>
        [Key]
        public int JogoId { get; set; }

        /// <summary>
        /// Nome do jogo.
        /// </summary>
        [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Género do jogo.
        /// </summary>
        [Required(ErrorMessage = "O género é obrigatório.")]
        [StringLength(50, ErrorMessage = "O género não pode ter mais de 50 caracteres.")]
        public string Genero { get; set; }

        /// <summary>
        /// Plataforma(s) em que o jogo está disponível.
        /// </summary>
        [Required(ErrorMessage = "A plataforma é obrigatória.")]
        [StringLength(100, ErrorMessage = "A plataforma não pode ter mais de 100 caracteres.")]
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Data de Lançamento")]
        [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Avaliações que o jogo recebeu.
        /// </summary>
        [ValidateNever]
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();


        /// <summary>
        /// Referência ao utilizador que criou o jogo.
        /// </summary>
        public string? UtilizadorId { get; set; }

        [ValidateNever]
        public IdentityUser? Utilizador { get; set; }


        /// <summary>
        /// Estatísticas agregadas do jogo.
        /// Relação 1:1 com a entidade Estatistica.
        /// </summary>
        [ValidateNever]
        public virtual Estatistica Estatistica { get; set; }

        [Display(Name = "Imagem (URL)")]
        [StringLength(300, ErrorMessage = "O URL da imagem não pode ter mais de 300 caracteres.")]
        public string? ImagemUrl { get; set; }


    }
}
