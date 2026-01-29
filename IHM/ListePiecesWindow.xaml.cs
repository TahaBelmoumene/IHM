using System.Windows;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ListePiecesWindow : Window
    {
        private GarageRepository _repo;

        // On modifie le constructeur pour accepter la VOITURE en plus de la catégorie
        public ListePiecesWindow(Categorie categorie, Motorisation voiture)
        {
            InitializeComponent();
            _repo = new GarageRepository();

            // On affiche un titre dynamique
            TxtTitre.Text = $"{categorie.Nom} (pour {voiture.Nom})";

            // C'EST ICI QUE LA MAGIE OPÈRE : 
            // On n'affiche que les pièces compatibles avec CETTE voiture dans CETTE catégorie
            GridPieces.ItemsSource = _repo.GetPiecesCompatibles(categorie.Id, voiture.Id);

            // Si la liste est vide, on peut prévenir l'utilisateur (optionnel)
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