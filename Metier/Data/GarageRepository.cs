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
        // Récupérer toutes les catégories
        public List<Categorie> GetCategories()
        {
            // On retourne juste la liste triée. 
            // L'initialisation se fait désormais UNIQUEMENT dans le constructeur.
            return _context.Categories.OrderBy(c => c.Nom).ToList();
        }
        public GarageRepository()
        {
            _context = new GarageContext();
            _context.Database.EnsureCreated(); // Crée le fichier vide s'il n'existe pas

            // C'est cette ligne qui remplit tout ton stock au premier lancement
            InitialiserArchitectureStock();
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

        // 1. CRÉER UN LIEN : Dire qu'une pièce est compatible avec une voiture
        public void AjouterCompatibilite(int pieceId, int motorisationId)
        {
            // On vérifie si le lien existe déjà pour éviter les doublons
            bool existeDeja = _context.Compatibilites
                                      .Any(c => c.PieceId == pieceId && c.MotorisationId == motorisationId);

            if (!existeDeja)
            {
                var lien = new Compatibilite { PieceId = pieceId, MotorisationId = motorisationId };
                _context.Compatibilites.Add(lien);
                _context.SaveChanges();
            }
        }

        // 2. CONSULTER : Récupérer les pièces d'une catégorie MAIS FILTRÉES pour une voiture
        public List<Piece> GetPiecesCompatibles(int categorieId, int motorisationId)
        {
            return _context.Compatibilites
                           .Where(c => c.MotorisationId == motorisationId && c.Piece.CategorieId == categorieId)
                           .Select(c => c.Piece) // On ne garde que la pièce, pas l'objet compatibilité
                           .OrderBy(p => p.Nom)
                           .ToList();
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

        public void AjouterPiece(string nom, decimal prix, int stock, int categorieId, int motorisationId) // Ajout du paramètre
        {
            // 1. Créer la pièce
            var nouvellePiece = new Piece
            {
                Nom = nom,
                Prix = prix,
                Stock = stock,
                CategorieId = categorieId
            };

            _context.Pieces.Add(nouvellePiece);
            _context.SaveChanges(); // Important : Cela génère l'ID de la nouvelle pièce

            // 2. Créer le lien de compatibilité immédiatement
            AjouterCompatibilite(nouvellePiece.Id, motorisationId);
        }

        public void InitialiserArchitectureStock()
        {
            // SÉCURITÉ : Si la base contient déjà des catégories, on ne touche à rien
            if (_context.Categories.Any()) return;

            // ====================================================================
            // 1. RAYON : FREINAGE (On garde celui qu'on a fait avant)
            // ====================================================================
            var freinage = new Categorie { Nom = "Freinage" };
            _context.Categories.Add(freinage);
            _context.SaveChanges();

            // -> Plaquettes
            var plaquettes = new Categorie { Nom = "Plaquettes de frein", ParentId = freinage.Id };
            _context.Categories.Add(plaquettes);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Plaquettes de frein avant", ParentId = plaquettes.Id });
            _context.Categories.Add(new Categorie { Nom = "Plaquettes de frein arrière", ParentId = plaquettes.Id });
            _context.Categories.Add(new Categorie { Nom = "Témoin d'usure de frein", ParentId = plaquettes.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit de montage plaquettes", ParentId = plaquettes.Id });

            // -> Disques
            var disques = new Categorie { Nom = "Disques de frein", ParentId = freinage.Id };
            _context.Categories.Add(disques);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Disques de frein avant", ParentId = disques.Id });
            _context.Categories.Add(new Categorie { Nom = "Disques de frein arrière", ParentId = disques.Id });
            _context.Categories.Add(new Categorie { Nom = "Vis de disque de frein", ParentId = disques.Id });
            _context.Categories.Add(new Categorie { Nom = "Déflecteur de disque", ParentId = disques.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit de freins à disques", ParentId = disques.Id });

            // -> Tambours
            var tambours = new Categorie { Nom = "Freins à tambours", ParentId = freinage.Id };
            _context.Categories.Add(tambours);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit de freins", ParentId = tambours.Id });
            _context.Categories.Add(new Categorie { Nom = "Mâchoires de frein", ParentId = tambours.Id });
            _context.Categories.Add(new Categorie { Nom = "Cylindre de roue", ParentId = tambours.Id });
            _context.Categories.Add(new Categorie { Nom = "Tambour de frein", ParentId = tambours.Id });

            // -> Hydraulique
            var hydraulique = new Categorie { Nom = "Freinage Hydraulique", ParentId = freinage.Id };
            _context.Categories.Add(hydraulique);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Étrier de frein", ParentId = hydraulique.Id });
            _context.Categories.Add(new Categorie { Nom = "Flexible de frein", ParentId = hydraulique.Id });
            _context.Categories.Add(new Categorie { Nom = "Maître-cylindre", ParentId = hydraulique.Id });
            _context.Categories.Add(new Categorie { Nom = "Liquide de frein", ParentId = hydraulique.Id });


            // ====================================================================
            // 2. RAYON : HUILES, LIQUIDES ET LUBRIFIANTS
            // ====================================================================
            var huiles = new Categorie { Nom = "Huiles, liquides et lubrifiants" };
            _context.Categories.Add(huiles);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Huile moteur", ParentId = huiles.Id });
            _context.Categories.Add(new Categorie { Nom = "Huile pour boîte", ParentId = huiles.Id });
            _context.Categories.Add(new Categorie { Nom = "Liquide de frein", ParentId = huiles.Id });
            _context.Categories.Add(new Categorie { Nom = "Liquide de refroidissement", ParentId = huiles.Id });


            // ====================================================================
            // 3. RAYON : FILTRATION
            // ====================================================================
            var filtration = new Categorie { Nom = "Filtration" };
            _context.Categories.Add(filtration);
            _context.SaveChanges();

            // -> Filtres
            var filtres = new Categorie { Nom = "Filtres", ParentId = filtration.Id };
            _context.Categories.Add(filtres);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Filtre à huile", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre à air", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre à carburant", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre d'habitacle", ParentId = filtres.Id });
            _context.Categories.Add(new Categorie { Nom = "Boitier de filtre à huile", ParentId = filtres.Id });

            // -> Accessoires
            var accFiltration = new Categorie { Nom = "Accessoires de filtration", ParentId = filtration.Id };
            _context.Categories.Add(accFiltration);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Bouchon de vidange", ParentId = accFiltration.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint de bouchon de vidange", ParentId = accFiltration.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint boitier filtre", ParentId = accFiltration.Id });

            // -> Autres
            var autresFiltres = new Categorie { Nom = "Autres filtres", ParentId = filtration.Id };
            _context.Categories.Add(autresFiltres);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Filtre hydraulique direction", ParentId = autresFiltres.Id });
            _context.Categories.Add(new Categorie { Nom = "Filtre ventilation carter", ParentId = autresFiltres.Id });


            // ====================================================================
            // 4. RAYON : PIÈCES MOTEUR (Le gros morceau !)
            // ====================================================================
            var moteur = new Categorie { Nom = "Pièces moteur" };
            _context.Categories.Add(moteur);
            _context.SaveChanges();

            // -> Distribution
            var distrib = new Categorie { Nom = "Distribution", ParentId = moteur.Id };
            _context.Categories.Add(distrib);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit de distribution", ParentId = distrib.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit distribution + Pompe à eau", ParentId = distrib.Id });
            _context.Categories.Add(new Categorie { Nom = "Courroie de distribution", ParentId = distrib.Id });
            _context.Categories.Add(new Categorie { Nom = "Galets (Enrouleur/Tendeur)", ParentId = distrib.Id });

            // -> Courroies d'accessoires
            var courroie = new Categorie { Nom = "Courroies", ParentId = moteur.Id };
            _context.Categories.Add(courroie);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Courroie d'accessoires", ParentId = courroie.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit courroie d'accessoires", ParentId = courroie.Id });
            _context.Categories.Add(new Categorie { Nom = "Poulie Damper", ParentId = courroie.Id });

            // -> Chaîne (Correction du doublon ici, on laisse la catégorie propre)
            var chaine = new Categorie { Nom = "Chaîne de distribution", ParentId = moteur.Id };
            _context.Categories.Add(chaine);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit chaîne de distribution", ParentId = chaine.Id });

            // -> Refroidissement (Le vrai !)
            var refroid = new Categorie { Nom = "Refroidissement", ParentId = moteur.Id };
            _context.Categories.Add(refroid);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Pompe à eau", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Radiateur moteur", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Thermostat", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Vase d'expansion", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Ventilateur", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Durite de refroidissement", ParentId = refroid.Id });
            _context.Categories.Add(new Categorie { Nom = "Intercooler", ParentId = refroid.Id });

            // -> Injection
            var injection = new Categorie { Nom = "Système d'injection", ParentId = moteur.Id };
            _context.Categories.Add(injection);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Injecteur (Diesel/Essence)", ParentId = injection.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint d'injecteur", ParentId = injection.Id });
            _context.Categories.Add(new Categorie { Nom = "Pompe à injection / HP", ParentId = injection.Id });
            _context.Categories.Add(new Categorie { Nom = "Rampe commune", ParentId = injection.Id });

            // -> Alimentation Carburant
            var alimCarb = new Categorie { Nom = "Alimentation carburant", ParentId = moteur.Id };
            _context.Categories.Add(alimCarb);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Pompe à carburant", ParentId = alimCarb.Id });
            _context.Categories.Add(new Categorie { Nom = "Régulateur de pression", ParentId = alimCarb.Id });
            _context.Categories.Add(new Categorie { Nom = "Bouchon de réservoir", ParentId = alimCarb.Id });

            // -> Alimentation Air
            var alimAir = new Categorie { Nom = "Alimentation d'air", ParentId = moteur.Id };
            _context.Categories.Add(alimAir);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Débitmètre d'air", ParentId = alimAir.Id });
            _context.Categories.Add(new Categorie { Nom = "Boîtier papillon", ParentId = alimAir.Id });
            _context.Categories.Add(new Categorie { Nom = "Vanne EGR / Admission", ParentId = alimAir.Id });
            _context.Categories.Add(new Categorie { Nom = "Durite d'admission", ParentId = alimAir.Id });

            // -> Turbo
            var turbo = new Categorie { Nom = "Turbo et suralimentation", ParentId = moteur.Id };
            _context.Categories.Add(turbo);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Turbo", ParentId = turbo.Id });
            _context.Categories.Add(new Categorie { Nom = "Durite de turbo", ParentId = turbo.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur pression turbo", ParentId = turbo.Id });

            // -> Culasse
            var culasse = new Categorie { Nom = "Culasse", ParentId = moteur.Id };
            _context.Categories.Add(culasse);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Joint de culasse", ParentId = culasse.Id });
            _context.Categories.Add(new Categorie { Nom = "Vis de culasse", ParentId = culasse.Id });
            _context.Categories.Add(new Categorie { Nom = "Arbre à cames", ParentId = culasse.Id });
            _context.Categories.Add(new Categorie { Nom = "Cache culbuteur", ParentId = culasse.Id });

            // -> Lubrification
            var lubrification = new Categorie { Nom = "Pièces lubrification", ParentId = moteur.Id };
            _context.Categories.Add(lubrification);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Carter d'huile", ParentId = lubrification.Id });
            _context.Categories.Add(new Categorie { Nom = "Pompe à huile", ParentId = lubrification.Id });
            _context.Categories.Add(new Categorie { Nom = "Jauge à huile", ParentId = lubrification.Id });
            _context.Categories.Add(new Categorie { Nom = "Reniflard", ParentId = lubrification.Id });

            // -> Soupapes
            var soupapes = new Categorie { Nom = "Soupapes moteur", ParentId = moteur.Id };
            _context.Categories.Add(soupapes);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Soupapes (Admission/Echappement)", ParentId = soupapes.Id });
            _context.Categories.Add(new Categorie { Nom = "Poussoir hydraulique", ParentId = soupapes.Id });
            _context.Categories.Add(new Categorie { Nom = "Culbuteur", ParentId = soupapes.Id });

            // -> Vilebrequin & Bas moteur
            var vilebrequin = new Categorie { Nom = "Vilebrequin & Piston", ParentId = moteur.Id };
            _context.Categories.Add(vilebrequin);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Vilebrequin", ParentId = vilebrequin.Id });
            _context.Categories.Add(new Categorie { Nom = "Coussinets (Bielle/Vilebrequin)", ParentId = vilebrequin.Id });
            _context.Categories.Add(new Categorie { Nom = "Piston & Segments", ParentId = vilebrequin.Id });
            _context.Categories.Add(new Categorie { Nom = "Bielle", ParentId = vilebrequin.Id });

            // -> Supports
            var supports = new Categorie { Nom = "Supports moteur", ParentId = moteur.Id };
            _context.Categories.Add(supports);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Support moteur", ParentId = supports.Id });
            _context.Categories.Add(new Categorie { Nom = "Support de boîte", ParentId = supports.Id });

            // -> Joints
            var joints = new Categorie { Nom = "Joints et bagues", ParentId = moteur.Id };
            _context.Categories.Add(joints);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Pochette de joints complète", ParentId = joints.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint spi (Vilebrequin/AAC)", ParentId = joints.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint collecteur", ParentId = joints.Id });

            // -> Capteurs
            var capteurs = new Categorie { Nom = "Capteurs & Relais moteur", ParentId = moteur.Id };
            _context.Categories.Add(capteurs);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Capteur PMH / Arbre à cames", ParentId = capteurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Débitmètre", ParentId = capteurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Sonde Lambda", ParentId = capteurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Sonde de température", ParentId = capteurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur pression", ParentId = capteurs.Id });

            // ====================================================================
            // RAYON 5 : DÉMARRAGE ET CHARGE
            // ====================================================================
            var demarrage = new Categorie { Nom = "Démarrage et charge" };
            _context.Categories.Add(demarrage);
            _context.SaveChanges();

            // -> Batterie (Direct)
            _context.Categories.Add(new Categorie { Nom = "Batterie voiture", ParentId = demarrage.Id });

            // -> Alternateurs
            var alternateurs = new Categorie { Nom = "Alternateurs et pièces", ParentId = demarrage.Id };
            _context.Categories.Add(alternateurs);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Alternateurs", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Poulie roue libre", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Poulie d'alternateur", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Balais d'alternateur", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Rotor d'alternateur", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Stator d'alternateur", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Régulateur d'alternateur", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Pont de diodes", ParentId = alternateurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation alternateur", ParentId = alternateurs.Id });

            // -> Allumage
            var allumage = new Categorie { Nom = "Bougies et pièces d'allumage", ParentId = demarrage.Id };
            _context.Categories.Add(allumage);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Bougie d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Bobine d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Faisceau d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Distributeur d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Tête d'allumeur", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Doigt allumeur", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Module d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Condensateur d'allumage", ParentId = allumage.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation allumeur", ParentId = allumage.Id });

            // -> Préchauffage
            var prechauffage = new Categorie { Nom = "Bougies et pièces préchauffage", ParentId = demarrage.Id };
            _context.Categories.Add(prechauffage);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Bougie de préchauffage", ParentId = prechauffage.Id });
            _context.Categories.Add(new Categorie { Nom = "Relais de préchauffage", ParentId = prechauffage.Id });
            _context.Categories.Add(new Categorie { Nom = "Bougie chauffage auxiliaire", ParentId = prechauffage.Id });

            // -> Démarreurs
            var demarreurs = new Categorie { Nom = "Démarreurs et pièces", ParentId = demarrage.Id };
            _context.Categories.Add(demarreurs);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Démarreur", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Solénoïde / Contacteur", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Balais de démarreur", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Lanceur / Pignon", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Induit / Inducteur", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Porte-balais", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation démarreur", ParentId = demarreurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur de batterie", ParentId = demarreurs.Id });

            // ====================================================================
            // RAYON 6 : EMBRAYAGE - BOÎTE DE VITESSE - TRANSMISSION
            // ====================================================================
            var transmission = new Categorie { Nom = "Embrayage - Boîte de vitesse - Transmission" };
            _context.Categories.Add(transmission);
            _context.SaveChanges();

            // -> Embrayage & Volant moteur
            var embrayage = new Categorie { Nom = "Embrayage & volant moteur", ParentId = transmission.Id };
            _context.Categories.Add(embrayage);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Volant moteur", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Butée d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Guide de butée", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Disque d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Fourchette d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Mécanisme d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Émetteur d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Récepteur d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit émetteur + récepteur", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Couronne dentée volant moteur", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Câble d'embrayage", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Boulon de volant moteur", ParentId = embrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Convertisseur de couple", ParentId = embrayage.Id });

            // -> Autres pièces d'embrayage
            var autresEmbrayage = new Categorie { Nom = "Autres pièces d'embrayage", ParentId = transmission.Id };
            _context.Categories.Add(autresEmbrayage);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Kit bagues d'étanchéité", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit assemblage émetteur/récepteur", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Pédale d'embrayage", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Tuyau d'embrayage", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Palier de fourchette", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint spi volant moteur", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Vis de plaque de pression", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation embrayage auto", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation volant moteur", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur pédale d'embrayage", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Tringle de sélection de vitesse", ParentId = autresEmbrayage.Id });
            _context.Categories.Add(new Categorie { Nom = "Limiteur de couple", ParentId = autresEmbrayage.Id });

            // -> Accessoires Boîte de vitesse
            var boite = new Categorie { Nom = "Accessoires de boîte de vitesse", ParentId = transmission.Id };
            _context.Categories.Add(boite);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Arbre d'entraînement différentiel", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Jeu de joints boîte manuelle", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint spi arbre principal", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint spi différentiel", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation levier vitesse", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Tringlerie de vitesse", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Câble de boîte de vitesse", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Support de boîte", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Radiateur d'huile de boîte", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation différentiel", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation boîte", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Croisillon satellite", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Douille levier de vitesse", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Jauge niveau huile boîte auto", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Calculateur boîte auto", ParentId = boite.Id });
            _context.Categories.Add(new Categorie { Nom = "Valve de commande boîte", ParentId = boite.Id });

            // -> Transmission & Cardan
            var cardan = new Categorie { Nom = "Transmission & Cardan", ParentId = transmission.Id };
            _context.Categories.Add(cardan);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Cardan de transmission", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Soufflet de cardan", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Tête de cardan", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Arbre de transmission (Pont)", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Palier central d'arbre", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Flector de transmission", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Amortisseur de vibration", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint d'étanchéité différentiel", ParentId = cardan.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation arbre intermédiaire", ParentId = cardan.Id });

            // ====================================================================
            // RAYON 7 : DIRECTION - SUSPENSION - ROULEMENT
            // ====================================================================
            var directionSuspension = new Categorie { Nom = "Direction - Suspension - Roulement" };
            _context.Categories.Add(directionSuspension);
            _context.SaveChanges();

            // -> Amortisseurs
            var amortisseurs = new Categorie { Nom = "Amortisseurs", ParentId = directionSuspension.Id };
            _context.Categories.Add(amortisseurs);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Amortisseur avant", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Amortisseur arrière", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Ressort de suspension", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Ensemble ressorts / amortisseurs", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Coupelle de suspension", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Roulement de coupelle", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit protection (Soufflet/Butée)", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Silent bloc d'amortisseur", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Lame de ressort", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Sphère de suspension", ParentId = amortisseurs.Id });
            _context.Categories.Add(new Categorie { Nom = "Suspension pneumatique", ParentId = amortisseurs.Id });

            // -> Direction
            var direction = new Categorie { Nom = "Direction", ParentId = directionSuspension.Id };
            _context.Categories.Add(direction);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Crémaillère de direction", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Colonne de direction", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Pompe de direction assistée", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Bocal de direction assistée", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Tuyau hydraulique", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Amortisseur de direction", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Cardan / Croisillon de direction", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur angle de braquage", ParentId = direction.Id });
            _context.Categories.Add(new Categorie { Nom = "Neiman / Antivol de direction", ParentId = direction.Id });

            // -> Rotules et Biellettes (Direction)
            var rotulesDir = new Categorie { Nom = "Rotules de direction", ParentId = directionSuspension.Id };
            _context.Categories.Add(rotulesDir);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Rotule de direction", ParentId = rotulesDir.Id });
            _context.Categories.Add(new Categorie { Nom = "Biellette de direction (Axiale)", ParentId = rotulesDir.Id });
            _context.Categories.Add(new Categorie { Nom = "Soufflet de direction", ParentId = rotulesDir.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit biellette + rotule", ParentId = rotulesDir.Id });

            // -> Pièces d'essieux et Suspension
            var essieux = new Categorie { Nom = "Pièces d'essieux & Suspension", ParentId = directionSuspension.Id };
            _context.Categories.Add(essieux);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Triangle de suspension", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Bras de suspension", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Rotule de suspension", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Silent bloc de triangle", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Barre stabilisatrice", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Biellette de barre stabilisatrice", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Silent bloc de barre stab", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Fusée / Pivot d'essieu", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Moyeu de roue", ParentId = essieux.Id });
            _context.Categories.Add(new Categorie { Nom = "Kit réparation train avant/arrière", ParentId = essieux.Id });

            // -> Roulements
            var roulements = new Categorie { Nom = "Roulements de roue", ParentId = directionSuspension.Id };
            _context.Categories.Add(roulements);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Roulement de roue avant", ParentId = roulements.Id });
            _context.Categories.Add(new Categorie { Nom = "Roulement de roue arrière", ParentId = roulements.Id });
            _context.Categories.Add(new Categorie { Nom = "Moyeu de roue complet", ParentId = roulements.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteur ABS (Intégré)", ParentId = roulements.Id });
            _context.Categories.Add(new Categorie { Nom = "Écrou de cardan / moyeu", ParentId = roulements.Id });

            // ====================================================================
            // RAYON 8 : OPTIQUE & ESSUIE-GLACE
            // ====================================================================
            var optique = new Categorie { Nom = "Optique & Essuie-glace" };
            _context.Categories.Add(optique);
            _context.SaveChanges();

            // -> Essuie-glaces
            var essuieGlaces = new Categorie { Nom = "Essuie-glaces", ParentId = optique.Id };
            _context.Categories.Add(essuieGlaces);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Balais d'essuie-glace avant", ParentId = essuieGlaces.Id });
            _context.Categories.Add(new Categorie { Nom = "Balais d'essuie-glace arrière", ParentId = essuieGlaces.Id });
            _context.Categories.Add(new Categorie { Nom = "Bras d'essuie-glace", ParentId = essuieGlaces.Id });
            _context.Categories.Add(new Categorie { Nom = "Moteur d'essuie-glace", ParentId = essuieGlaces.Id });
            _context.Categories.Add(new Categorie { Nom = "Pompe de lave-glace", ParentId = essuieGlaces.Id });
            _context.Categories.Add(new Categorie { Nom = "Réservoir de lave-glace", ParentId = essuieGlaces.Id });

            // -> Optiques et Phares
            var phares = new Categorie { Nom = "Optiques et phares", ParentId = optique.Id };
            _context.Categories.Add(phares);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Phare avant", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Feu arrière", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Clignotant", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Antibrouillard", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Feu de plaque", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Feu de stop additionnel", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Répétiteur latéral", ParentId = phares.Id });
            _context.Categories.Add(new Categorie { Nom = "Correcteur de portée", ParentId = phares.Id });

            // -> Ampoules
            var ampoules = new Categorie { Nom = "Ampoules", ParentId = optique.Id };
            _context.Categories.Add(ampoules);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Ampoule H7/H4/H1", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Ampoule Xénon", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Ampoule de clignotant", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Ampoule de feu stop/arrière", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Ampoule de plaque", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Ampoule habitacle", ParentId = ampoules.Id });
            _context.Categories.Add(new Categorie { Nom = "Coffret d'ampoules", ParentId = ampoules.Id });

            // ====================================================================
            // RAYON 9 : CLIMATISATION
            // ====================================================================
            var clim = new Categorie { Nom = "Climatisation" };
            _context.Categories.Add(clim);
            _context.SaveChanges();

            // -> Pièces de climatisation (Regroupement)
            var piecesClim = new Categorie { Nom = "Pièces de climatisation", ParentId = clim.Id };
            _context.Categories.Add(piecesClim);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Compresseur de clim", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Condenseur de clim", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Bouteille déshydratante", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Évaporateur", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Détendeur", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Pressostat / Capteur", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Tuyau de climatisation", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Ventilateur habitacle (Pulseur)", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Résistance de chauffage", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Embrayage de compresseur", ParentId = piecesClim.Id });
            _context.Categories.Add(new Categorie { Nom = "Joints de clim", ParentId = piecesClim.Id });


            // ====================================================================
            // RAYON 10 : CARROSSERIE
            // ====================================================================
            var carrosserie = new Categorie { Nom = "Carrosserie" };
            _context.Categories.Add(carrosserie);
            _context.SaveChanges();

            // -> Rétroviseurs
            var retro = new Categorie { Nom = "Rétroviseur", ParentId = carrosserie.Id };
            _context.Categories.Add(retro);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Rétroviseur extérieur", ParentId = retro.Id });
            _context.Categories.Add(new Categorie { Nom = "Coque / Revêtement", ParentId = retro.Id });
            _context.Categories.Add(new Categorie { Nom = "Miroir de rétroviseur", ParentId = retro.Id });
            _context.Categories.Add(new Categorie { Nom = "Mécanisme intérieur", ParentId = retro.Id });
            _context.Categories.Add(new Categorie { Nom = "Rétroviseur extérieur cabine", ParentId = retro.Id });
            _context.Categories.Add(new Categorie { Nom = "Rétroviseur intérieur", ParentId = retro.Id });

            // -> Pare-chocs
            var parechocs = new Categorie { Nom = "Pare-chocs", ParentId = carrosserie.Id };
            _context.Categories.Add(parechocs);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Pare-chocs", ParentId = parechocs.Id });
            _context.Categories.Add(new Categorie { Nom = "Support de pare-chocs", ParentId = parechocs.Id });
            _context.Categories.Add(new Categorie { Nom = "Grille de pare-chocs", ParentId = parechocs.Id });
            _context.Categories.Add(new Categorie { Nom = "Capteurs de recul / stationnement", ParentId = parechocs.Id });
            _context.Categories.Add(new Categorie { Nom = "Enjoliveur de pare-chocs", ParentId = parechocs.Id });
            _context.Categories.Add(new Categorie { Nom = "Baguettes protectrices", ParentId = parechocs.Id });

            // -> Portières et Latérale
            var portieres = new Categorie { Nom = "Portières et latérale", ParentId = carrosserie.Id };
            _context.Categories.Add(portieres);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Portière voiture", ParentId = portieres.Id });
            _context.Categories.Add(new Categorie { Nom = "Aile", ParentId = portieres.Id });
            _context.Categories.Add(new Categorie { Nom = "Baguette latérale", ParentId = portieres.Id });
            _context.Categories.Add(new Categorie { Nom = "Poignée de porte", ParentId = portieres.Id });

            // -> Carrosserie Avant
            var avant = new Categorie { Nom = "Éléments carrosserie avant", ParentId = carrosserie.Id };
            _context.Categories.Add(avant);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Insonorisant moteur / Cache sous moteur", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Agrafes de fixation", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Tôle de phare", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Berceau moteur", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Grille de calandre", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Cerclage antibrouillard", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Baguettes de phare", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Butoir de capot", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Spoiler / Lame", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Support de plaque", ParentId = avant.Id });
            _context.Categories.Add(new Categorie { Nom = "Support de phare / Calandre", ParentId = avant.Id });

            // -> Carrosserie Arrière
            var arriere = new Categorie { Nom = "Éléments carrosserie arrière", ParentId = carrosserie.Id };
            _context.Categories.Add(arriere);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Hayon / Coffre", ParentId = arriere.Id });
            _context.Categories.Add(new Categorie { Nom = "Vérin de coffre", ParentId = arriere.Id });

            // -> Joints
            var joints_carrosserie = new Categorie { Nom = "Joints carrosserie", ParentId = carrosserie.Id };
            _context.Categories.Add(joints);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Joint de porte", ParentId = joints_carrosserie.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint de pare-brise", ParentId = joints_carrosserie.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint de vitre arrière", ParentId = joints_carrosserie.Id });
            _context.Categories.Add(new Categorie { Nom = "Joint lèche-vitre", ParentId = joints_carrosserie.Id });

            // ====================================================================
            // RAYON 11 : JANTES ET ÉQUIPEMENTS ROUE
            // ====================================================================
            var roues = new Categorie { Nom = "Jantes et équipements roue" };
            _context.Categories.Add(roues);
            _context.SaveChanges();

            // -> Jantes
            var jantes = new Categorie { Nom = "Jantes", ParentId = roues.Id };
            _context.Categories.Add(jantes);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Jantes Alu", ParentId = jantes.Id });
            _context.Categories.Add(new Categorie { Nom = "Jantes Tôle", ParentId = jantes.Id });
            _context.Categories.Add(new Categorie { Nom = "Enjoliveurs", ParentId = jantes.Id });

            // -> Équipements
            var equipRoues = new Categorie { Nom = "Équipements roue", ParentId = roues.Id };
            _context.Categories.Add(equipRoues);
            _context.SaveChanges();
            _context.Categories.Add(new Categorie { Nom = "Boulon / Écrou de roue", ParentId = equipRoues.Id });
            _context.Categories.Add(new Categorie { Nom = "Antivol de roue", ParentId = equipRoues.Id });
            _context.Categories.Add(new Categorie { Nom = "Valve de roue", ParentId = equipRoues.Id });
            _context.Categories.Add(new Categorie { Nom = "Chaînes à neige", ParentId = equipRoues.Id });

            // ====================================================================
            // 5. LES AUTRES RAYONS (Pour le futur)
            // ====================================================================
            var autresRayons = new List<string>
            {
        "Démarrage et charge",
        "Embrayage - Boîte de vitesse - Transmission",
        "Direction - Suspension - Roulement",
        "Chauffage et ventilation",
        "Échappement",
        "Optique & Essuie-glace",
        "Climatisation",
        "Carrosserie",
        "Transport",
        "Habitacle",
        "Jante et équipements roue"
    };
    
            foreach (var nom in autresRayons)
            {
                _context.Categories.Add(new Categorie { Nom = nom });
            }

            _context.SaveChanges();
        }
    }
}
