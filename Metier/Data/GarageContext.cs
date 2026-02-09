    using Metier.Entities;
    using Microsoft.EntityFrameworkCore;
    
namespace Metier.Entities
{ 
    public class GarageContext : DbContext
    {
    public DbSet<Client> Clients { get; set; }
    public DbSet<VehiculeClient> VehiculesClients { get; set; }
    public DbSet<Marque> Marques { get; set; }
    public DbSet<Modele> Modeles { get; set; }
    public DbSet<Generation> Generations { get; set; }
    public DbSet<Motorisation> Motorisations { get; set; }

    public DbSet<Categorie> Categories { get; set; }
    public DbSet<Piece> Pieces { get; set; }
    public DbSet<Compatibilite> Compatibilites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        optionsBuilder.UseSqlite("Data Source=garage.db");
        }
    }
}