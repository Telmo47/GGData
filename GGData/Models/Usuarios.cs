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
        /// Identificador do Usuário
        /// </summary>
        [Key]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nome do Usuário
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        /// <summary>
        /// Password do utilizador (normalmente não guardamos password em texto claro, mas aqui está)
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
        /// Tipo de usuário da conta, pode ser "Critico" ou "Utilizador"
        /// </summary>
        [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Nome de utilizador para autenticação (normalmente o email)
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Instituição para utilizadores do tipo Crítico (opcional, usado para verificação)
        /// </summary>
        public string Instituicao { get; set; }

        /// <summary>
        /// Website profissional para críticos (opcional)
        /// </summary>
        public string WebsiteProfissional { get; set; }

        /// <summary>
        /// Descrição profissional para críticos (opcional)
        /// </summary>
        public string DescricaoProfissional { get; set; }
    }
}
