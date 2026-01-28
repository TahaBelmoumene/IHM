using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Metier.Entities
{
    [Table("Marques")]
    public class Marque
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
    }
}
