using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<Usuarios, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Avaliacao> Avaliacao { get; set; }
    public DbSet<Jogo> Jogos { get; set; }
    public DbSet<Estatistica> Estatistica { get; set; }
    public DbSet<Usuarios> Usuarios { get; set; }

    public DbSet<Genero> Generos { get; set; }
    public DbSet<JogoGenero> JogoGeneros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Jogo)
            .WithMany(j => j.Avaliacoes)
            .HasForeignKey(a => a.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estatistica>()
            .HasOne(e => e.Jogo)
            .WithOne(j => j.Estatistica)
            .HasForeignKey<Estatistica>(e => e.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuração da relação muitos-para-muitos Jogo-Genero
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
