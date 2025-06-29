using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGData.Models
{
    [Table("AspNetUsers")]  // Mapeia para a tabela padrão do Identity
    public class Usuarios : IdentityUser<int>
    {
        public string Nome { get; set; }
        public DateTime DataRegistro { get; set; }
        public string TipoUsuario { get; set; }
        public string? Instituicao { get; set; }
        public string? WebsiteProfissional { get; set; }
        public string? DescricaoProfissional { get; set; }

        [ValidateNever]
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
