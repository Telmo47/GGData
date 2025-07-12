using System.ComponentModel.DataAnnotations;

namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Modelo de registo utilizado para criar uma nova conta de utilizador no sistema.
    /// Suporta campos opcionais para perfis profissionais (ex: críticos).
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Email do utilizador. Será usado como nome de login.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string Email { get; set; }

        /// <summary>
        /// Palavra-passe de acesso.
        /// </summary>
        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Confirmação da palavra-passe.
        /// </summary>
        [Required(ErrorMessage = "A confirmação da palavra-passe é obrigatória.")]
        [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Tipo de utilizador: "Critico" ou "Utilizador".
        /// </summary>
        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        [StringLength(20, ErrorMessage = "O tipo de utilizador deve ter no máximo 20 caracteres.")]
        public string TipoUsuario { get; set; }

        /// <summary>
        /// (Opcional) Nome da instituição profissional (ex: jornal, blog, etc).
        /// </summary>
        public string? Instituicao { get; set; }

        /// <summary>
        /// (Opcional) Website profissional do utilizador.
        /// </summary>
        [Url(ErrorMessage = "O website não é válido.")]
        public string? WebsiteProfissional { get; set; }

        /// <summary>
        /// (Opcional) Descrição do perfil profissional.
        /// </summary>
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? DescricaoProfissional { get; set; }
    }
}
