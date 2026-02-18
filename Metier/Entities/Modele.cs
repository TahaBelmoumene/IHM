using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Modeles")]
    public class Modele
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public int MarqueId { get; set; }

        [ForeignKey("MarqueId")]
        public Marque? Marque { get; set; }
    }
}