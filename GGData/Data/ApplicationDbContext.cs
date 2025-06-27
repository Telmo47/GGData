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

            // Relação Avaliacao <-> Jogo (1:n)
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Jogo)
                .WithMany(j => j.Avaliacoes)
                .HasForeignKey(a => a.JogoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação Estatistica <-> Jogo (1:1)
            modelBuilder.Entity<Estatistica>()
                .HasOne(e => e.Jogo)
                .WithOne(j => j.Estatistica)
                .HasForeignKey<Estatistica>(e => e.JogoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração da precisão das propriedades decimal para evitar truncamento
            modelBuilder.Entity<Estatistica>(entity =>
            {
                entity.Property(e => e.MediaNotaCriticos).HasPrecision(5, 2);
                entity.Property(e => e.MediaNotaUtilizadores).HasPrecision(5, 2);
                entity.Property(e => e.TempoMedioJogo).HasPrecision(10, 2);
            });
        }
    }
}
