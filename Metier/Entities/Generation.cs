using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Generations")]
    public class Generation
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public int AnneeDebut { get; set; }
        public int AnneeFin { get; set; }

        public int ModeleId { get; set; }

        [ForeignKey("ModeleId")]
        public Modele? Modele { get; set; }
    }
}