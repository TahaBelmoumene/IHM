using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Pieces")]
    public class Piece
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } // Ex: "Filtre à huile"
        public decimal Prix { get; set; }
        public int Stock { get; set; }

        // Lien avec la catégorie
        public int CategorieId { get; set; }
        [ForeignKey("CategorieId")]
        public Categorie Categorie { get; set; }
    }
}