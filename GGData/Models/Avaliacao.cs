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
        /// ???
        /// </summary>

        public DateTime DataReview { get; set; }

        /// <summary>
        /// ???
        /// </summary>

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
