using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<Utilizadores, IdentityRole<int>, int>
{
    /// <summary>
    /// Construtor que recebe opções para configurar o contexto da base de dados.
    /// </summary>
    /// <param name="options">Opções do DbContext</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>
    /// Conjunto de avaliações feitas pelos utilizadores.
    /// </summary>
    public DbSet<Avaliacao> Avaliacao { get; set; }

    /// <summary>
    /// Conjunto de jogos.
    /// </summary>
    public DbSet<Jogo> Jogos { get; set; }

    /// <summary>
    /// Conjunto de estatísticas relacionadas a jogos.
    /// </summary>
    public DbSet<Estatistica> Estatistica { get; set; }

    /// <summary>
    /// Conjunto de utilizadores (extendendo IdentityUser).
    /// </summary>
    public DbSet<Utilizadores> Utilizadores { get; set; }

    /// <summary>
    /// Conjunto de géneros de jogos.
    /// </summary>
    public DbSet<Genero> Generos { get; set; }

    /// <summary>
    /// Tabela de relação muitos-para-muitos entre Jogos e Géneros.
    /// </summary>
    public DbSet<JogoGenero> JogoGeneros { get; set; }

    /// <summary>
    /// Configura as relações entre entidades e regras de exclusão.
    /// </summary>
    /// <param name="modelBuilder">Construtor do modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Avaliação tem relação 1-N com Jogo, com restrição na exclusão
        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Jogo)
            .WithMany(j => j.Avaliacoes)
            .HasForeignKey(a => a.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Estatística tem relação 1-1 com Jogo, com restrição na exclusão
        modelBuilder.Entity<Estatistica>()
            .HasOne(e => e.Jogo)
            .WithOne(j => j.Estatistica)
            .HasForeignKey<Estatistica>(e => e.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relação muitos-para-muitos entre Jogo e Genero via JogoGenero
        modelBuilder.Entity<JogoGenero>()
            .HasKey(jg => new { jg.JogoId, jg.GeneroId });

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
