using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Representa um utilizador do sistema, extendendo a classe IdentityUser com chave int.
    /// </summary>
    [Table("AspNetUsers")]  // Mapeia explicitamente para a tabela do Identity
    public class Utilizadores : IdentityUser<int>
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data de registo do utilizador no sistema.
        /// </summary>
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Tipo de utilizador (ex: "Administrador", "Crítico", "Utilizador").
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Instituição associada ao utilizador (opcional).
        /// </summary>
        public string? Instituicao { get; set; }

        /// <summary>
        /// URL do website profissional do utilizador (opcional).
        /// </summary>
        public string? WebsiteProfissional { get; set; }

        /// <summary>
        /// Descrição profissional do utilizador (opcional).
        /// </summary>
        public string? DescricaoProfissional { get; set; }

        /// <summary>
        /// Coleção de avaliações feitas pelo utilizador.
        /// </summary>
        [ValidateNever]
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
