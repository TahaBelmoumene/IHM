using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Motorisations")]
    public class Motorisation
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Carburant { get; set; } = string.Empty;

        public int GenerationId { get; set; }

        [ForeignKey("GenerationId")]
        public Generation? Generation { get; set; }
    }
}