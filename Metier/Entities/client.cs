using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        public int Id { get; set; }

        // CORRECTION : On initialise avec "" pour éviter l'erreur de constructeur
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
    }
}