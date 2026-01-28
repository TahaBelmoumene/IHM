using Metier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metier.Data
{
    public class GarageRepository
    {
        private GarageContext _context;

        // Récupérer toutes les catégories (Moteur, Freinage, etc.)
        public List<Categorie> GetCategories()
        {
            // Si la table est vide, on crée quelques fausses catégories pour tester !
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Categorie { Nom = "Moteur & Distribution" });
                _context.Categories.Add(new Categorie { Nom = "Freinage" });
                _context.Categories.Add(new Categorie { Nom = "Filtration & Huile" });
                _context.Categories.Add(new Categorie { Nom = "Direction & Suspension" });
                _context.Categories.Add(new Categorie { Nom = "Carrosserie" });
                _context.SaveChanges();
            }

            return _context.Categories.OrderBy(c => c.Nom).ToList();
        }
        public GarageRepository()
        {
            _context = new GarageContext();
            _context.Database.EnsureCreated(); // Sécurité
        }

        public List<Marque> GetMarques()
        {
            return _context.Marques.OrderBy(m => m.Nom).ToList();
        }

        public List<Modele> GetModeles(int marqueId)
        {
            return _context.Modeles.Where(m => m.MarqueId == marqueId).OrderBy(m => m.Nom).ToList();
        }

        public List<Generation> GetGenerations(int modeleId)
        {
            return _context.Generations.Where(g => g.ModeleId == modeleId).OrderBy(g => g.AnneeDebut).ToList();
        }

        public List<Motorisation> GetMoteurs(int generationId)
        {
            return _context.Motorisations.Where(m => m.GenerationId == generationId).OrderBy(m => m.Nom).ToList();
        }
    }
}
