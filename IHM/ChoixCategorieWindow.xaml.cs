using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ChoixCategorieWindow : Window
    {
        private GarageRepository _repo;
        private Motorisation _voitureChoisie;

        // Historique pour savoir d'où on vient (pour le bouton Retour)
        private Stack<Categorie> _historique = new Stack<Categorie>();

        private Categorie _categorieEnCours = null; // Null = On est à la racine (les Rayons)

        public ChoixCategorieWindow(Motorisation voiture)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _voitureChoisie = voiture;

            TxtTitreVoiture.Text = $"Pièces pour : {voiture.Nom}";

            // Démarrage : on charge les rayons principaux (Niveau 0)
            ChargerNiveau(null);
        }

        private void ChargerNiveau(Categorie parent)
        {
            _categorieEnCours = parent;
            List<Categorie> listeA_Afficher;

            if (parent == null)
            {
                // On est tout en haut : on affiche les Rayons (Freinage, Moteur...)
                listeA_Afficher = _repo.GetRayonsPrincipaux();
                TxtAriane.Text = "Rayons Principaux";
                BtnRetour.Visibility = Visibility.Collapsed; // Pas de retour possible
            }
            else
            {
                // On est dans un dossier : on affiche ses enfants
                listeA_Afficher = _repo.GetSousCategories(parent.Id);
                TxtAriane.Text = parent.Nom; // Titre = Nom du dossier actuel
                BtnRetour.Visibility = Visibility.Visible;
            }

            LstCategories.ItemsSource = listeA_Afficher;
        }

        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie catSelectionnee)
            {
                // On regarde si cette catégorie contient des sous-catégories
                var sousCats = _repo.GetSousCategories(catSelectionnee.Id);

                if (sousCats.Count > 0)
                {
                    // CAS 1 : C'est un DOSSIER -> On descend dedans
                    _historique.Push(_categorieEnCours); // On mémorise la page d'avant
                    ChargerNiveau(catSelectionnee);      // On affiche la nouvelle page
                }
                else
                {
                    // CAS 2 : C'est une FIN (Feuille) -> On affiche les pièces
                    ListePiecesWindow fenetre = new ListePiecesWindow(catSelectionnee, _voitureChoisie);
                    fenetre.ShowDialog();
                }
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            if (_historique.Count > 0)
            {
                // On revient au dossier précédent
                var parentPrecedent = _historique.Pop();
                ChargerNiveau(parentPrecedent);
            }
            else
            {
                // Sécurité : retour à la racine
                ChargerNiveau(null);
            }
        }
    }
}