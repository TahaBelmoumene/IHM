using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("VehiculesClients")]
    public class VehiculeClient
    {
        [Key]
        public int Id { get; set; }
        public string Plaque { get; set; } // Ex: AA-123-BB

        public int MotorisationId { get; set; }
        [ForeignKey("MotorisationId")]
        public Motorisation Motorisation { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}