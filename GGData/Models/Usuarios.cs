using System;
using System.ComponentModel.DataAnnotations;

namespace GGData.Models
{
    /// <summary>
    /// Usuários que entrarão no site
    /// </summary>
    public class Usuarios
    {
        /// <summary>
        /// Identificador do Úsuário
        /// </summary>
        [Key]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nome do Usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        /// <summary>
        /// Password do utilizador
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        /// <summary>
        /// Data de registro do utilizador
        /// </summary>
        [Required(ErrorMessage = "A data de registo é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Email do utilizador
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email inserido não é válido.")]
        public string Email { get; set; }

        /// <summary>
        /// tipo de usuário da conta, pode ser um critico ou um usuário normal
        /// </summary>
        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        public string tipoUsuario { get; set; }
    }
}
