using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("LignesFacture")]
    public class LigneFacture
    {
        [Key]
        public int Id { get; set; }

        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public string NomPiece { get; set; } = string.Empty;

        public int FactureId { get; set; }
        [ForeignKey("FactureId")]
        public Facture? Facture { get; set; }

        public int? PieceId { get; set; }
        [ForeignKey("PieceId")]
        public Piece? Piece { get; set; }
    }
}