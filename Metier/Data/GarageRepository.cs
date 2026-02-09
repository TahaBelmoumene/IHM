using System.Collections.Generic;
using System.Linq;
using Metier.Entities;
using Microsoft.EntityFrameworkCore;

namespace Metier.Data
{
    public class GarageRepository
    {
        private readonly GarageContext _context;

        public GarageRepository()
        {
            _context = new GarageContext();
            _context.Database.EnsureCreated();
            InitialiserArchitectureStock();
            InitialiserArchitectureMarques();
        }

        public List<Categorie> GetCategories() => _context.Categories.OrderBy(c => c.Nom).ToList();
        public List<Marque> GetMarques() => _context.Marques.OrderBy(m => m.Nom).ToList();
        public List<Modele> GetModeles(int marqueId) => _context.Modeles.Where(m => m.MarqueId == marqueId).OrderBy(m => m.Nom).ToList();
        public List<Generation> GetGenerations(int modeleId) => _context.Generations.Where(g => g.ModeleId == modeleId).OrderBy(g => g.AnneeDebut).ToList();
        public List<Motorisation> GetMoteurs(int generationId) => _context.Motorisations.Where(m => m.GenerationId == generationId).OrderBy(m => m.Nom).ToList();

        public List<Categorie> GetRayonsPrincipaux() => _context.Categories.Where(c => c.ParentId == null).OrderBy(c => c.Nom).ToList();
        public List<Categorie> GetSousCategories(int parentId) => _context.Categories.Where(c => c.ParentId == parentId).OrderBy(c => c.Nom).ToList();

        public List<Piece> GetPiecesParCategorie(int categorieId) => _context.Pieces.Where(p => p.CategorieId == categorieId).OrderBy(p => p.Nom).ToList();

        public List<Marque> GetOrigines() => _context.Marques.Where(m => m.ParentId == null).OrderBy(m => m.Nom).ToList();
        public List<Marque> GetMarquesParOrigine(int origineId) => _context.Marques.Where(m => m.ParentId == origineId).OrderBy(m => m.Nom).ToList();

        public List<Piece> GetPiecesCompatibles(int categorieId, int motorisationId) =>
            _context.Compatibilites
                    .Where(c => c.MotorisationId == motorisationId && c.Piece.CategorieId == categorieId)
                    .Select(c => c.Piece)
                    .OrderBy(p => p.Nom)
                    .ToList();

        public List<Client> GetClients() => _context.Clients.OrderBy(c => c.Nom).ToList();

        public void AjouterCategorie(string nom, int? parentId = null)
        {
            _context.Categories.Add(new Categorie { Nom = nom, ParentId = parentId });
            _context.SaveChanges();
        }

        public void AjouterMarque(string nom, int? parentId = null)
        {
            var existe = _context.Marques.Any(m => m.Nom == nom && m.ParentId == parentId);

            if (!existe)
            {
                _context.Marques.Add(new Marque { Nom = nom, ParentId = parentId });
                _context.SaveChanges();
            }
        }

        public void AjouterModele(string nom, int marqueId)
        {
            _context.Modeles.Add(new Modele { Nom = nom, MarqueId = marqueId });
            _context.SaveChanges();
        }

        public void AjouterGeneration(string nom, int annee, int modeleId)
        {
            _context.Generations.Add(new Generation { Nom = nom, AnneeDebut = annee, ModeleId = modeleId });
            _context.SaveChanges();
        }

        public void AjouterMoteur(string nom, string carburant, int generationId)
        {
            _context.Motorisations.Add(new Motorisation { Nom = nom, Carburant = carburant, GenerationId = generationId });
            _context.SaveChanges();
        }

        public void AjouterClient(string nom, string prenom, string tel)
        {
            _context.Clients.Add(new Client { Nom = nom, Prenom = prenom, Telephone = tel });
            _context.SaveChanges();
        }

        public void AjouterPiece(string nom, decimal prix, int stock, string etat, int categorieId, int motorisationId)
        {
            var nouvellePiece = new Piece
            {
                Nom = nom,
                Prix = prix,
                Stock = stock,
                Etat = etat,
                CategorieId = categorieId
            };

            _context.Pieces.Add(nouvellePiece);
            _context.SaveChanges();
            AjouterCompatibilite(nouvellePiece.Id, motorisationId);
        }

        public void ModifierPiece(int id, string nouveauNom, decimal nouveauPrix, int nouveauStock, string nouvelEtat)
        {
            var piece = _context.Pieces.FirstOrDefault(p => p.Id == id);
            if (piece != null)
            {
                piece.Nom = nouveauNom;
                piece.Prix = nouveauPrix;
                piece.Stock = nouveauStock;
                piece.Etat = nouvelEtat;
                _context.SaveChanges();
            }
        }

        public void AjouterCompatibilite(int pieceId, int motorisationId)
        {
            var existeDeja = _context.Compatibilites.Any(c => c.PieceId == pieceId && c.MotorisationId == motorisationId);
            if (!existeDeja)
            {
                _context.Compatibilites.Add(new Compatibilite { PieceId = pieceId, MotorisationId = motorisationId });
                _context.SaveChanges();
            }
        }

        public VehiculeClient? GetInfosVehiculeClient(string plaque) =>
            _context.VehiculesClients
                    .Include(v => v.Client)
                    .Include(v => v.Motorisation)
                        .ThenInclude(m => m.Generation)
                        .ThenInclude(g => g.Modele)
                        .ThenInclude(md => md.Marque)
                    .FirstOrDefault(v => v.Plaque == plaque);

        public Motorisation? GetVoitureParPlaque(string plaque) =>
            _context.VehiculesClients
                    .Include(v => v.Motorisation)
                        .ThenInclude(m => m.Generation)
                        .ThenInclude(g => g.Modele)
                        .ThenInclude(md => md.Marque)
                    .FirstOrDefault(v => v.Plaque == plaque)
                    ?.Motorisation;

        public void EnregistrerPlaque(string plaque, int motorisationId, int? clientId = null)
        {
            var existant = _context.VehiculesClients.FirstOrDefault(v => v.Plaque == plaque);
            if (existant == null)
            {
                _context.VehiculesClients.Add(new VehiculeClient
                {
                    Plaque = plaque.ToUpper(),
                    MotorisationId = motorisationId,
                    ClientId = clientId
                });
                _context.SaveChanges();
            }
        }

        public List<object> GetPiecesAvecVehicule(int categorieId)
        {
            var query = from p in _context.Pieces
                        join c in _context.Compatibilites on p.Id equals c.PieceId
                        join m in _context.Motorisations on c.MotorisationId equals m.Id
                        where p.CategorieId == categorieId
                        select new
                        {
                            PieceOriginale = p,
                            NomPiece = p.Nom,
                            Prix = p.Prix,
                            Stock = p.Stock,
                            Etat = p.Etat,
                            NomVehicule = m.Generation.Modele.Marque.Nom + " " + m.Generation.Modele.Nom + " " + m.Nom
                        };

            return query.ToList<object>();
        }

        public void AjouterPackDemarrage(int motorisationId)
        {
            var listeStandard = new List<(string Categorie, string NomPiece, decimal Prix)>
            {
                ("Filtre à huile", "Filtre à huile Standard", 15),
                ("Filtre à air", "Filtre à air Standard", 20),
                ("Filtre habitacle", "Filtre habitacle Charbon", 25),
                ("Plaquettes Avant", "Jeu de plaquettes Avant", 45),
                ("Plaquettes Arrière", "Jeu de plaquettes Arrière", 35),
                ("Disques Avant", "Jeu de disques Avant", 80),
                ("Batterie", "Batterie 12V 60Ah", 90),
                ("Alternateur", "Alternateur Standard", 150),
                ("Démarreur", "Démarreur Standard", 120)
            };

            foreach (var item in listeStandard)
            {
                var categorie = _context.Categories.FirstOrDefault(c => c.Nom == item.Categorie);

                if (categorie != null)
                {
                    bool existe = _context.Compatibilites
                        .Any(c => c.MotorisationId == motorisationId && c.Piece.Nom == item.NomPiece);

                    if (!existe)
                    {
                        AjouterPiece(item.NomPiece, item.Prix, 1, "Neuf", categorie.Id, motorisationId);
                    }
                }
            }
        }

        public void InitialiserArchitectureMarques()
        {
            if (!_context.Marques.Any())
            {
                var francaise = new Marque { Nom = "Française" };
                var allemande = new Marque { Nom = "Allemande" };
                var italienne = new Marque { Nom = "Italienne" };
                var japonaise = new Marque { Nom = "Japonaise" };
                var americaine = new Marque { Nom = "Américaine" };
                var autre = new Marque { Nom = "Autre" };

                _context.Marques.AddRange(francaise, allemande, italienne, japonaise, americaine, autre);
                _context.SaveChanges();

                _context.Marques.Add(new Marque { Nom = "PSA", ParentId = francaise.Id });
                _context.Marques.Add(new Marque { Nom = "Renault", ParentId = francaise.Id });

                _context.Marques.Add(new Marque { Nom = "Volkswagen", ParentId = allemande.Id });
                _context.Marques.Add(new Marque { Nom = "Audi", ParentId = allemande.Id });
                _context.Marques.Add(new Marque { Nom = "Mercedes", ParentId = allemande.Id });
                _context.Marques.Add(new Marque { Nom = "BMW", ParentId = allemande.Id });

                _context.Marques.Add(new Marque { Nom = "Fiat", ParentId = italienne.Id });
                _context.Marques.Add(new Marque { Nom = "Alfa Romeo", ParentId = italienne.Id });
                _context.Marques.Add(new Marque { Nom = "Ferrari", ParentId = italienne.Id });

                _context.Marques.Add(new Marque { Nom = "Toyota", ParentId = japonaise.Id });
                _context.Marques.Add(new Marque { Nom = "Nissan", ParentId = japonaise.Id });
                _context.Marques.Add(new Marque { Nom = "Honda", ParentId = japonaise.Id });

                _context.Marques.Add(new Marque { Nom = "Ford", ParentId = americaine.Id });
                _context.Marques.Add(new Marque { Nom = "Tesla", ParentId = americaine.Id });
                _context.Marques.Add(new Marque { Nom = "Chevrolet", ParentId = americaine.Id });

                _context.Marques.Add(new Marque { Nom = "Divers", ParentId = autre.Id });

                _context.SaveChanges();
            }
            else
            {
                if (!_context.Marques.Any(m => m.Nom == "Autre" && m.ParentId == null))
                {
                    var autre = new Marque { Nom = "Autre" };
                    _context.Marques.Add(autre);
                    _context.SaveChanges();
                    _context.Marques.Add(new Marque { Nom = "Divers", ParentId = autre.Id });
                    _context.SaveChanges();
                }
            }
        }

        public void InitialiserArchitectureStock()
        {
            if (_context.Categories.Any()) return;

            var freinage = new Categorie { Nom = "Freinage" };
            _context.Categories.Add(freinage); _context.SaveChanges();

            var plaquettes = new Categorie { Nom = "Plaquettes de frein", ParentId = freinage.Id };
            _context.Categories.Add(plaquettes); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Plaquettes Avant", ParentId = plaquettes.Id });
            _context.Categories.Add(new Categorie { Nom = "Plaquettes Arrière", ParentId = plaquettes.Id });

            var disques = new Categorie { Nom = "Disques de frein", ParentId = freinage.Id };
            _context.Categories.Add(disques); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Disques Avant", ParentId = disques.Id });
            _context.Categories.Add(new Categorie { Nom = "Disques Arrière", ParentId = disques.Id });

            var filtration = new Categorie { Nom = "Filtration" };
            _context.Categories.Add(filtration); _context.SaveChanges();
            var filtres = new Categorie { Nom = "Filtres", ParentId = filtration.Id };
            _context.Categories.Add(filtres); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Filtre à huile", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre à air", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre à carburant", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre habitacle", ParentId = filtres.Id });

            var direction = new Categorie { Nom = "Direction - Suspension" };
            _context.Categories.Add(direction); _context.SaveChanges();

            var susp = new Categorie { Nom = "Suspension", ParentId = direction.Id };
            _context.Categories.Add(susp); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Amortisseurs Avant", ParentId = susp.Id });
            _context.Categories.Add(new Categorie { Nom = "Amortisseurs Arrière", ParentId = susp.Id });
            _context.Categories.Add(new Categorie { Nom = "Ressort de suspension", ParentId = susp.Id });
            _context.Categories.Add(new Categorie { Nom = "Coupelles", ParentId = susp.Id });

            var dir = new Categorie { Nom = "Direction", ParentId = direction.Id };
            _context.Categories.Add(dir); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Rotule de direction", ParentId = dir.Id });
            _context.Categories.Add(new Categorie { Nom = "Crémaillère", ParentId = dir.Id });
            _context.Categories.Add(new Categorie { Nom = "Biellette", ParentId = dir.Id });

            var transmission = new Categorie { Nom = "Embrayage - Boîte" };
            _context.Categories.Add(transmission); _context.SaveChanges();

            var embrayage = new Categorie { Nom = "Embrayage", ParentId = transmission.Id };
            _context.Categories.Add(embrayage); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Volant moteur", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Butée", ParentId = embrayage.Id });

            var trans = new Categorie { Nom = "Transmission", ParentId = transmission.Id };
            _context.Categories.Add(trans); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Cardan", ParentId = trans.Id });
            _context.Categories.Add(new Categorie { Nom = "Soufflet", ParentId = trans.Id });

            var moteur = new Categorie { Nom = "Pièces Moteur" };
            _context.Categories.Add(moteur); _context.SaveChanges();

            var distrib = new Categorie { Nom = "Distribution", ParentId = moteur.Id };
            _context.Categories.Add(distrib); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit Distribution", ParentId = distrib.Id });
            _context.Categories.Add(new Categorie { Nom = "Pompe à eau", ParentId = distrib.Id });

            var inj = new Categorie { Nom = "Injection", ParentId = moteur.Id };
            _context.Categories.Add(inj); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Injecteur", ParentId = inj.Id });
            _context.Categories.Add(new Categorie { Nom = "Turbo", ParentId = inj.Id });

            var elec = new Categorie { Nom = "Démarrage & Charge" };
            _context.Categories.Add(elec); _context.SaveChanges();
            var demarreur = new Categorie { Nom = "Démarrage", ParentId = elec.Id };
            _context.Categories.Add(demarreur); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Batterie", ParentId = demarreur.Id });
            _context.Categories.Add(new Categorie { Nom = "Alternateur", ParentId = demarreur.Id });
            _context.Categories.Add(new Categorie { Nom = "Démarreur", ParentId = demarreur.Id });

            var echappement = new Categorie { Nom = "Échappement" };
            _context.Categories.Add(echappement); _context.SaveChanges();
            var pot = new Categorie { Nom = "Silencieux & Tuyaux", ParentId = echappement.Id };
            _context.Categories.Add(pot); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Silencieux Arrière", ParentId = pot.Id });
            _context.Categories.Add(new Categorie { Nom = "Catalyseur", ParentId = pot.Id });
            _context.Categories.Add(new Categorie { Nom = "Vanne EGR", ParentId = pot.Id });

            var roues = new Categorie { Nom = "Roues & Jantes" };
            _context.Categories.Add(roues); _context.SaveChanges();
            var jante = new Categorie { Nom = "Jantes", ParentId = roues.Id };
            _context.Categories.Add(jante); _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Jante Tôle", ParentId = jante.Id });
            _context.Categories.Add(new Categorie { Nom = "Jante Alu", ParentId = jante.Id });

            _context.SaveChanges();
        }
    }
}