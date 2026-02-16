using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ModifierPieceWindow : Window
    {
        private GarageRepository _repo;
        private Piece _pieceAModifier;

        public ModifierPieceWindow(Piece piece)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _pieceAModifier = piece;

            // Pré-remplissage des champs
            TxtNom.Text = piece.Nom;
            TxtPrix.Text = piece.Prix.ToString(); // Affiche avec la virgule locale
            TxtStock.Text = piece.Stock.ToString();

            // Sélection automatique de l'état
            foreach (ComboBoxItem item in CboEtat.Items)
            {
                if (item.Content.ToString() == piece.Etat)
                {
                    CboEtat.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validations de base
            if (string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                MessageBox.Show("Le nom ne peut pas être vide.");
                return;
            }

            if (!decimal.TryParse(TxtPrix.Text.Replace(".", ","), out decimal prix))
            {
                MessageBox.Show("Le prix est invalide (utilisez des chiffres).");
                return;
            }

            if (!int.TryParse(TxtStock.Text, out int stock))
            {
                MessageBox.Show("Le stock doit être un nombre entier.");
                return;
            }

            string etat = (CboEtat.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Neuf";

            // Sauvegarde en base de données via le Repository
            _repo.ModifierPiece(_pieceAModifier.Id, TxtNom.Text, prix, stock, etat);

            MessageBox.Show("Modification enregistrée avec succès !");
            this.Close();
        }
    }
}