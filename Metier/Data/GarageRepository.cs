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
        // 1. Récupérer uniquement les Rayons principaux (ceux qui n'ont pas de parent)
        public List<Categorie> GetRayonsPrincipaux()
        {
            return _context.Categories
                           .Where(c => c.ParentId == null)
                           .OrderBy(c => c.Nom)
                           .ToList();
        }

        // 2. Récupérer les sous-catégories d'un parent
        public List<Categorie> GetSousCategories(int parentId)
        {
            return _context.Categories
                           .Where(c => c.ParentId == parentId)
                           .OrderBy(c => c.Nom)
                           .ToList();
        }

        // 3. Ajouter une catégorie (Racine ou Enfant)
        public void AjouterCategorie(string nom, int? parentId = null)
        {
            var cat = new Categorie { Nom = nom, ParentId = parentId };
            _context.Categories.Add(cat);
            _context.SaveChanges();
        }
        public List<Piece> GetPiecesParCategorie(int categorieId)
        {
            return _context.Pieces
                           .Where(p => p.CategorieId == categorieId)
                           .OrderBy(p => p.Nom)
                           .ToList();
        }
        // Méthode pour remplir la BDD avec des fausses données de test
        // 1. Ajouter Marque
        public void AjouterMarque(string nom)
        {
            _context.Marques.Add(new Marque { Nom = nom });
            _context.SaveChanges();
        }

        // 2. Ajouter Modèle
        public void AjouterModele(string nom, int marqueId)
        {
            _context.Modeles.Add(new Modele { Nom = nom, MarqueId = marqueId });
            _context.SaveChanges();
        }

        // 3. Ajouter Génération (ex: Clio 4, année 2012)
        public void AjouterGeneration(string nom, int annee, int modeleId)
        {
            _context.Generations.Add(new Generation { Nom = nom, AnneeDebut = annee, ModeleId = modeleId });
            _context.SaveChanges();
        }

        // 4. Ajouter Motorisation
        public void AjouterMoteur(string nom, string carburant, int generationId)
        {
            _context.Motorisations.Add(new Motorisation { Nom = nom, Carburant = carburant, GenerationId = generationId });
            _context.SaveChanges();
        }

        public void AjouterPiece(string nom, decimal prix, int stock, int categorieId)
        {
            var nouvellePiece = new Piece
            {
                Nom = nom,
                Prix = prix,
                Stock = stock,
                CategorieId = categorieId
            };

            _context.Pieces.Add(nouvellePiece);
            _context.SaveChanges();
        }
    }
}
