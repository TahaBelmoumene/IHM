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

        public ChoixCategorieWindow(Motorisation voiture)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _voitureChoisie = voiture;

            TxtTitreVoiture.Text = $"Recherche pour : {voiture.Nom}";

            // On ne charge que le premier niveau (Rayons) au démarrage
            ChargerNiveau(null);
        }

        private void ChargerNiveau(int? parentId)
        {
            // Utilise les méthodes existantes de votre Repository
            var liste = (parentId == null)
                ? _repo.GetRayonsPrincipaux()
                : _repo.GetSousCategories(parentId.Value);

            LstCategories.ItemsSource = liste;
        }

        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie selection)
            {
                // On vérifie s'il y a des enfants (Familles ou Types)
                var enfants = _repo.GetSousCategories(selection.Id);

                if (enfants.Count > 0)
                {
                    // Si oui, on affiche le niveau suivant
                    TxtTitreVoiture.Text = $"Rayon : {selection.Nom}";
                    LstCategories.ItemsSource = enfants;
                }
                else
                {
                    // Si non (c'est le niveau final), on affiche les pièces
                    ListePiecesWindow fenetre = new ListePiecesWindow(selection, _voitureChoisie);
                    fenetre.ShowDialog();
                }
            }
        }
    }
}