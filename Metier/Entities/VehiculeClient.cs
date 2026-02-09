using System;
using System.Collections.Generic;
using System.Text;

namespace Metier.Entities
{
    public class VehiculeClient
    {
        public int Id { get; set; }
        public string Plaque { get; set; }

        public int MotorisationId { get; set; }
        public Motorisation Motorisation { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}