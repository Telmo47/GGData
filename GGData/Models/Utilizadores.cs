using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    /// <summary>
    /// Classe que representa um utilizador do sistema,
    /// extendendo a funcionalidade do IdentityUser para incluir dados adicionais.
    /// </summary>
    [Table("AspNetUsers")]  // Mapeia esta entidade para a tabela padrão do Identity no banco de dados
    public class Utilizadores : IdentityUser<int>
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Data em que o utilizador foi registado no sistema.
        /// </summary>
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Tipo do utilizador, e.g., "Administrador", "Critico", "Utilizador".
        /// </summary>
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Instituição a que o utilizador pertence (opcional).
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
        /// Esta propriedade é ignorada na validação do modelo para evitar loops.
        /// </summary>
        [ValidateNever]
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
