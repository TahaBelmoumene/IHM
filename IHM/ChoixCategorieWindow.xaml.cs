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

            TxtTitreVoiture.Text = $"Pièces pour : {voiture.Nom}";

            LstCategories.ItemsSource = _repo.GetCategories();
        }

        
        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie categorieSelectionnee)
            {
                
                ListePiecesWindow fenetre = new ListePiecesWindow(categorieSelectionnee, _voitureChoisie);
                fenetre.ShowDialog();
            }
        }
    }
}