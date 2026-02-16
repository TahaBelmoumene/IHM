using System.Windows;
using System.Windows.Input;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ChoixCategorieWindow : Window
    {
        private GarageRepository _repo;
        private Motorisation? _voitureChoisie; // Le ? rend la variable nullable

        // Le paramètre a une valeur par défaut "null"
        public ChoixCategorieWindow(Motorisation? voiture = null)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _voitureChoisie = voiture;

            if (_voitureChoisie != null)
            {
                TxtTitreVoiture.Text = $"Recherche pour : {_voitureChoisie.Nom}";
            }
            else
            {
                TxtTitreVoiture.Text = "Mode Inventaire Global (Toutes les pièces)";
            }

            ChargerNiveau(null);
        }

        private void ChargerNiveau(int? parentId)
        {
            var liste = (parentId == null)
                ? _repo.GetRayonsPrincipaux()
                : _repo.GetSousCategories(parentId.Value);

            LstCategories.ItemsSource = liste;
        }

        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie selection)
            {
                var enfants = _repo.GetSousCategories(selection.Id);

                if (enfants.Count > 0)
                {
                    // Mise à jour du titre selon le contexte
                    string contexte = _voitureChoisie != null ? $"({_voitureChoisie.Nom})" : "";
                    TxtTitreVoiture.Text = $"Rayon : {selection.Nom} {contexte}";

                    LstCategories.ItemsSource = enfants;
                }
                else
                {
                    // On ouvre la liste des pièces en passant la catégorie et la voiture (qui peut être null)
                    ListePiecesWindow fenetre = new ListePiecesWindow(selection, _voitureChoisie);
                    fenetre.ShowDialog();
                }
            }
        }
    }
}