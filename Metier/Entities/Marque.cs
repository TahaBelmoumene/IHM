using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Marques")]
    public class Marque
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Marque? Parent { get; set; }
    }
}