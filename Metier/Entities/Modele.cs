using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Metier.Entities
{
    [Table("Modeles")]
    public class Modele
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public int MarqueId { get; set; }
        [ForeignKey("MarqueId")]
        public Marque Marque { get; set; }
    }
}
