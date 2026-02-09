namespace Metier.Entities
{
    public class Piece
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public int Stock { get; set; }
        public string Etat { get; set; } 
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }
    }
}