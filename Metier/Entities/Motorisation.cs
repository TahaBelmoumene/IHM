using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Metier.Entities
{
    [Table("Motorisations")]
    public class Motorisation
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Carburant { get; set; }
        public int GenerationId { get; set; }
        [ForeignKey("GenerationId")]
        public Generation Generation { get; set; }
    }
}
