using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metier.Entities
{
    [Table("Factures")]
    public class Facture
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateEmission { get; set; }
        public decimal Total { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        // Liste des articles achetés
        public List<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();
    }
}