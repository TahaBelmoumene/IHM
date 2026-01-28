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

        // Lien vers le parent (ex: "Plaquettes" a pour parent "Freinage")
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Categorie? Parent { get; set; }

        // Liste des enfants (facultatif mais pratique pour EF)
        public List<Categorie> SousCategories { get; set; } = new List<Categorie>();
    }
}