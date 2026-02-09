using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Compatibilites")]
    public class Compatibilite
    {
        [Key]
        public int Id { get; set; }

        public int PieceId { get; set; }
        [ForeignKey("PieceId")]
        public Piece Piece { get; set; }

        public int MotorisationId { get; set; }
        [ForeignKey("MotorisationId")]
        public Motorisation Motorisation { get; set; }
    }
}