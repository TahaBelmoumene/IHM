using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Metier.Entities
{
    [Table("Categories")]
    public class Categorie
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Categorie? Parent { get; set; }

        public List<Categorie> SousCategories { get; set; } = new List<Categorie>();
    }
}