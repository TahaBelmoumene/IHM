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
                    TxtTitreVoiture.Text = $"Rayon : {selection.Nom}";
                    LstCategories.ItemsSource = enfants;
                }
                else
                {
                    ListePiecesWindow fenetre = new ListePiecesWindow(selection, _voitureChoisie);
                    fenetre.ShowDialog();
                }
            }
        }
    }
}