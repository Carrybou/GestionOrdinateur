using GestionOrdinateurs.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Context;

public class ParcInformatiqueContext : DbContext //Context de la base de données
{

    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    public DbSet<Ordinateur> Ordinateurs { get; set; }
    public DbSet<Reparation> Reparations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)//Configuration des entités
    {
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(ordinateur => ordinateur.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(ordinateur => ordinateur.Reparations)
            .WithOne(reparation => reparation.UnOrdinateur)
            .HasForeignKey(reparation => reparation.OrdinateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reparation>() //relation entre Reparation et Ordinateur
            .HasKey(reparation => reparation.Id);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Login)
            .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
    }

    


}