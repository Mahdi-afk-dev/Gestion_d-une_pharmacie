using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace GestionPharmacie;

public class PharmacieContext : DbContext
{
    public DbSet<Personne> Personnes { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Pharmacien> Pharmaciens { get; set; }
    public DbSet<Medecin> Medecins { get; set; }
    public DbSet<Fournisseur> Fournisseurs { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Lot> Lots { get; set; }
    public DbSet<Ordonnance> Ordonnances { get; set; }
    public DbSet<LigneOrdonnance> LignesOrdonnance { get; set; }
    public DbSet<Vente> Ventes { get; set; }
    public DbSet<LigneVente> LignesVente { get; set; }
    public DbSet<LivraisonFournisseur> LivraisonsFournisseurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string cheminBdd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pharmacie.db");
        optionsBuilder.UseSqlite($"Data Source={cheminBdd}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fournisseur>()
            .Navigation(f => f.Medicaments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Ordonnance>()
            .Navigation(o => o.Lignes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Vente>()
            .Navigation(v => v.Lignes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}