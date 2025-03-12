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
        public string Nome { get; set; }

        /// <summary>
        /// Password do utilizador
        /// </summary>
        public string Senha { get; set; }

        /// <summary>
        /// Data de registro do utilizador
        /// </summary>
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Email do utilizador
        /// </summary>
        public string Email { get; set; }



    }
}
