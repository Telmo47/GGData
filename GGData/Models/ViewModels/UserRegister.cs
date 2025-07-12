using System.ComponentModel.DataAnnotations;

namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Modelo para o registo de novos utilizadores.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        /// <summary>
        /// Email do utilizador.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Palavra-passe do utilizador.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Confirmação da palavra-passe (deve coincidir com Password).
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Tipo do utilizador (ex: Crítico, Utilizador, Administrador).
        /// </summary>
        [Required]
        [StringLength(20)]
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Instituição profissional do utilizador (opcional).
        /// </summary>
        public string? Instituicao { get; set; } = null;

        /// <summary>
        /// Website profissional do utilizador (opcional).
        /// </summary>
        public string? WebsiteProfissional { get; set; } = null;

        /// <summary>
        /// Descrição profissional do utilizador (opcional).
        /// </summary>
        public string? DescricaoProfissional { get; set; } = null;
    }
}
