using GGData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GGData.Data
{
    /// <summary>
    /// Esta classe representa a Base de Dados associada ao projeto
    /// Se houver mais bases de dados, irão haver tantas classes
    /// deste tipo, quantas as necessárias
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabelas

        /// <summary>
        /// Tabela Avaliação
        /// </summary>
        public DbSet<Avaliacao> Avaliacao { get; set; }

        /// <summary>
        /// Tabela Estatistica
        /// </summary>
        public DbSet<Estatistica> Estatistica { get; set; }

        /// <summary>
        /// Tabela Jogo
        /// </summary>
        public DbSet<Jogo> Jogo { get; set; }

        /// <summary>
        /// Tabela Usuários
        /// </summary>
        public DbSet<Usuarios> Usuarios { get; set; }

        /// <summary>
        /// Configuração das relações e regras adicionais
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação Avaliacao <-> Jogo
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Jogo)
                .WithMany(j => j.Avaliacoes)
                .HasForeignKey(a => a.JogoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração da relação Estatistica <-> Jogo
            modelBuilder.Entity<Estatistica>()
                .HasOne(e => e.Jogo)
                .WithMany(j => j.Estatisticas)
                .HasForeignKey(e => e.JogoID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
