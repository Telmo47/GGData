using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
                .HasForeignKey(e => e.JogoId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------------------------------
            // Seed de Role e Utilizador Admin para autenticação
            // -----------------------------------------------

            // Criar a role Administrador
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "a", // ID arbitrário
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR"
                }
            );

            // Criar o utilizador admin
            var hasher = new PasswordHasher<IdentityUser>();
            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = "admin", // ID arbitrário
                    UserName = "admin@mail.pt",
                    NormalizedUserName = "ADMIN@MAIL.PT",
                    Email = "admin@mail.pt",
                    NormalizedEmail = "ADMIN@MAIL.PT",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa"), // password segura
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            // Associar o utilizador admin à role Administrador
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "a",
                    UserId = "admin"
                }
            );
        }
    }
}
