using System;
using System.Collections.Generic;

namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) utilizado para criar ou transferir dados de um jogo,
    /// especialmente ao comunicar com APIs ou formulários.
    /// </summary>
    public class JogoDTO
    {
        /// <summary>
        /// Nome do jogo.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Plataforma onde o jogo está disponível (ex: PC, PS5, Xbox).
        /// </summary>
        public string Plataforma { get; set; }

        /// <summary>
        /// Data de lançamento do jogo.
        /// </summary>
        public DateTime DataLancamento { get; set; }

        /// <summary>
        /// Lista de géneros associados ao jogo (ex: Aventura, Ação, RPG).
        /// </summary>
        public List<string> Generos { get; set; } = new List<string>();
    }
}
