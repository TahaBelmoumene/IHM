using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ListePiecesWindow : Window
    {
        private GarageRepository _repo;
        private Categorie _categorieEnCours;
        private Motorisation? _voitureEnCours;

        public ListePiecesWindow(Categorie categorie, Motorisation? voiture = null)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _categorieEnCours = categorie;
            _voitureEnCours = voiture;

            ChargerPieces();
        }

        private void ChargerPieces()
        {
            // Mise à jour du titre
            TxtTitre.Text = _voitureEnCours != null
                ? $"{_categorieEnCours.Nom} (pour {_voitureEnCours.Nom})"
                : $"{_categorieEnCours.Nom} (Tout le stock)";

            // Sélection de la méthode de récupération selon le mode
            if (_voitureEnCours == null)
            {
                // Mode inventaire : on prend tout ce qui est dans la catégorie
                GridPieces.ItemsSource = _repo.GetPiecesParCategorie(_categorieEnCours.Id);
            }
            else
            {
                // Mode recherche client : on ne prend que les pièces compatibles
                GridPieces.ItemsSource = _repo.GetPiecesCompatibles(_categorieEnCours.Id, _voitureEnCours.Id);
            }

            if (GridPieces.Items.Count == 0)
            {
                // Petit message discret dans la fenêtre de debug ou optionnel à l'utilisateur
                // MessageBox.Show("Aucune pièce trouvée."); 
            }
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'élément lié au bouton cliqué
            if (sender is Button btn && btn.DataContext is Piece pieceSelectionnee)
            {
                ModifierPieceWindow fenetre = new ModifierPieceWindow(pieceSelectionnee);
                fenetre.ShowDialog();

                // Rafraîchir la liste après la modification
                _repo = new GarageRepository(); // On recrée le repo pour être sûr d'avoir les données fraîches
                ChargerPieces();
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}