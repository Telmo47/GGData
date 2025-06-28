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
    }
}
