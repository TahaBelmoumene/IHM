using System.Windows;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ListePiecesWindow : Window
    {
        private GarageRepository _repo;

        public ListePiecesWindow(Categorie categorie, Motorisation voiture)
        {
            InitializeComponent();
            _repo = new GarageRepository();

            TxtTitre.Text = $"{categorie.Nom} (pour {voiture.Nom})";

            
            GridPieces.ItemsSource = _repo.GetPiecesCompatibles(categorie.Id, voiture.Id);

            if (GridPieces.Items.Count == 0)
            {
                MessageBox.Show("Aucune pièce compatible trouvée pour ce véhicule dans ce rayon.");
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}