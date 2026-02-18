using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Categories")]
    public class Categorie
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Categorie? Parent { get; set; }
    }
}