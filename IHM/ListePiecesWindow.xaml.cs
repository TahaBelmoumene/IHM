using System.Windows;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ListePiecesWindow : Window
    {
        private GarageRepository _repo;

        public ListePiecesWindow(Categorie categorie)
        {
            InitializeComponent();
            _repo = new GarageRepository();

            TxtTitre.Text = $"Rayon : {categorie.Nom}";

            // On charge les pièces de cette catégorie
            GridPieces.ItemsSource = _repo.GetPiecesParCategorie(categorie.Id);
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}