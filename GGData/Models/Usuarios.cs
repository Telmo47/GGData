using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GGData.Models
{
    /// <summary>
    /// Representa um utilizador registado no sistema.
    /// </summary>
    public class Usuarios
    {
        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        [Key]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nome completo do utilizador.
        /// Pode ser um nome extenso, até 150 caracteres.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome não pode ter mais de 150 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Palavra-passe do utilizador.
        /// Nota: idealmente não se deve guardar em texto claro.
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        /// <summary>
        /// Data de registo do utilizador no sistema.
        /// </summary>
        [Required(ErrorMessage = "A data de registo é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Email do utilizador, validado para formato correto.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email inserido não é válido.")]
        public string Email { get; set; }

        /// <summary>
        /// Tipo do utilizador, como "Critico" ou "Utilizador".
        /// </summary>
        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        [StringLength(20)]
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Nome de utilizador para autenticação (normalmente o email).
        /// </summary>
        [Required(ErrorMessage = "O nome de utilizador (UserName) é obrigatório.")]
        [StringLength(50, ErrorMessage = "O UserName não pode ter mais de 50 caracteres.")]
        public string UserName { get; set; }

        /// <summary>
        /// Instituição para utilizadores do tipo Crítico (opcional, usado para verificação).
        /// </summary>
        public string? Instituicao { get; set; }

        /// <summary>
        /// Website profissional para críticos (opcional).
        /// </summary>
        public string? WebsiteProfissional { get; set; }

        /// <summary>
        /// Descrição profissional para críticos (opcional).
        /// </summary>
        public string? DescricaoProfissional { get; set; }

        /// <summary>
        /// Coleção de avaliações feitas pelo utilizador.
        /// Cada utilizador pode ter várias avaliações, uma por jogo.
        /// </summary>
        [ValidateNever]
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
