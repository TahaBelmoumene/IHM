using Metier.Entities;
using Microsoft.EntityFrameworkCore;

public class GarageContext : DbContext
{
    public DbSet<Marque> Marques { get; set; }
    public DbSet<Modele> Modeles { get; set; }
    public DbSet<Generation> Generations { get; set; }
    public DbSet<Motorisation> Motorisations { get; set; }

    // --- AJOUTE CES DEUX LIGNES ---
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<Piece> Pieces { get; set; }
    // -----------------------------

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // REMPLACE LE TEMPS DU TEST :
        // optionsBuilder.UseSqlite("Data Source=garage.db");

        // PAR TON CHEMIN RÉEL (Mets bien le @ devant les guillemets) :
        optionsBuilder.UseSqlite("Data Source=garage.db");
    }
}