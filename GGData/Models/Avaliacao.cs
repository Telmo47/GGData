using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{

    /// <summary>
    /// Avaliação dada a cada jogo
    /// </summary>
    public class Avaliacao
    {


        /// <summary>
        /// Id da avaliação dada ao jogo
        /// </summary>
        [Key]
        public int AvaliacaoId { get; set; }

        /// <summary>
        /// Nota dada individualmente por cada usuário ao jogo
        /// </summary>
        public int Nota { get; set; }

        /// <summary>
        /// Comentários dados pelos usuários
        /// </summary>

        public string Comentarios { get; set; }

        /// <summary>
        /// Reviews dos jogos
        /// </summary>

        public DateTime DataReview { get; set; }

        /// <summary>
        /// Tipo de usuário que deu a avaliação ao jogo, pode ser um crítico ou um usuário normal
        /// </summary>

        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [StringLength(20)]
        [Display(Name = "Tipo de Usuário")]
        public string TipoUsuario{ get; set; }


        //FKs


        /// <summary>
        /// Chave forasteira com referência aos usuários
        /// </summary>
        [ForeignKey(nameof(Usuarios))]
        public int UsuariosID { get; set; }
        public Usuarios Usuario{ get; set; }

        /// <summary>
        /// Chave forasteira com referência ao jogo
        /// </summary>
        [ForeignKey(nameof(Jogo))]
        public int JogoID { get; set; }

        public Jogo Jogo { get; set; }
    }
}
