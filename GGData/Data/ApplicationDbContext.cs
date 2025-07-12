using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Contexto da base de dados principal da aplicação, estendendo IdentityDbContext para suporte a autenticação e gestão de utilizadores.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<Utilizadores, IdentityRole<int>, int>
{
    /// <summary>
    /// Construtor que aceita opções de configuração do contexto.
    /// </summary>
    /// <param name="options">Opções do DbContext, normalmente configuradas no Startup/Program.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Tabelas da base de dados
    public DbSet<Avaliacao> Avaliacao { get; set; }
    public DbSet<Jogo> Jogos { get; set; }
    public DbSet<Estatistica> Estatistica { get; set; }
    public DbSet<Utilizadores> Utilizadores { get; set; }
    public DbSet<Genero> Generos { get; set; }
    public DbSet<JogoGenero> JogoGeneros { get; set; }

    /// <summary>
    /// Configuração do modelo de dados com relações e constraints.
    /// </summary>
    /// <param name="modelBuilder">Builder usado para configurar entidades e relações.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura relação 1-N entre Avaliação e Jogo (um jogo pode ter várias avaliações)
        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Jogo)
            .WithMany(j => j.Avaliacoes)
            .HasForeignKey(a => a.JogoId)
            .OnDelete(DeleteBehavior.Restrict); // Impede cascata de delete

        // Configura relação 1-1 entre Estatística e Jogo
        modelBuilder.Entity<Estatistica>()
            .HasOne(e => e.Jogo)
            .WithOne(j => j.Estatistica)
            .HasForeignKey<Estatistica>(e => e.JogoId)
            .OnDelete(DeleteBehavior.Restrict); // Também evita cascata

        // Configura relação N-N entre Jogo e Género com tabela intermédia JogoGenero
        modelBuilder.Entity<JogoGenero>()
            .HasKey(jg => new { jg.JogoId, jg.GeneroId }); // Chave composta

        modelBuilder.Entity<JogoGenero>()
            .HasOne(jg => jg.Jogo)
            .WithMany(j => j.JogoGeneros)
            .HasForeignKey(jg => jg.JogoId);

        modelBuilder.Entity<JogoGenero>()
            .HasOne(jg => jg.Genero)
            .WithMany(g => g.JogoGeneros)
            .HasForeignKey(jg => jg.GeneroId);
    }
}
