using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Metier.Entities
{
    [Table("Generations")]
    public class Generation
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public int AnneeDebut { get; set; }
        public int ModeleId { get; set; }
        [ForeignKey("ModeleId")]
        public Modele Modele { get; set; }
    }
}
