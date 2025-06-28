using System.ComponentModel.DataAnnotations;

namespace GGData.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoUsuario { get; set; }

        public string? Instituicao { get; set; }
        public string? WebsiteProfissional { get; set; }
        public string? DescricaoProfissional { get; set; }
    }
}
