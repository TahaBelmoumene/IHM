using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;
using Microsoft.Win32; // Pour SaveFileDialog
using IHM.Services;    // Pour notre FactureExporter
namespace IHM
{
    // Petite classe locale pour l'affichage dans le DataGrid
    public class LignePanier
    {
        public Piece PieceOriginale { get; set; }
        public string NomPiece { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal TotalLigne => Quantite * PrixUnitaire;
    }

    public partial class CreationFactureWindow : Window
    {
        private GarageRepository _repo;
        private List<LignePanier> _panier; // Liste temporaire avant sauvegarde

        public CreationFactureWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _panier = new List<LignePanier>();

            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            CboClients.ItemsSource = _repo.GetClients();
            CboPieces.ItemsSource = _repo.GetAllPieces();
        }

        private void BtnAjouterLigne_Click(object sender, RoutedEventArgs e)
        {
            if (CboPieces.SelectedItem is Piece pieceSelectionnee && int.TryParse(TxtQte.Text, out int qte) && qte > 0)
            {
                // Vérifier le stock
                if (pieceSelectionnee.Stock < qte)
                {
                    MessageBox.Show($"Stock insuffisant ! (Dispo : {pieceSelectionnee.Stock})");
                    return;
                }

                // Ajouter au panier
                _panier.Add(new LignePanier
                {
                    PieceOriginale = pieceSelectionnee,
                    NomPiece = pieceSelectionnee.Nom,
                    PrixUnitaire = pieceSelectionnee.Prix,
                    Quantite = qte
                });

                RafraichirPanier();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une pièce et une quantité valide.");
            }
        }
        private void BtnValiderFacture_Click(object sender, RoutedEventArgs e)
        {
            if (CboClients.SelectedItem is Client client)
            {
                if (_panier.Count == 0)
                {
                    MessageBox.Show("Le panier est vide !");
                    return;
                }

                // 1. Sauvegarde en Base de Données
                var articles = _panier.Select(l => (l.PieceOriginale, l.Quantite)).ToList();

                // MODIFICATION ICI : On fait retourner la facture créée par le repo pour avoir son ID
                // (Il faudra adapter légèrement le Repo, voir plus bas)
                var nouvelleFacture = _repo.CreerFacture(client, articles);

                // 2. Génération du PDF
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Fichier PDF (*.pdf)|*.pdf",
                    FileName = $"Facture_{nouvelleFacture.Id}_{client.Nom}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        FactureExporter.GenererPdf(nouvelleFacture, saveFileDialog.FileName);

                        // Optionnel : Ouvrir le PDF directement
                        var p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true };
                        p.Start();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la création du PDF : {ex.Message}");
                    }
                }

                MessageBox.Show("Facture enregistrée et exportée !");
                this.Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un client.");
            }
        }

        private void BtnSupprimerLigne_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LignePanier ligne)
            {
                _panier.Remove(ligne);
                RafraichirPanier();
            }
        }

        private void RafraichirPanier()
        {
            GridPanier.ItemsSource = null;
            GridPanier.ItemsSource = _panier;

            decimal total = _panier.Sum(l => l.TotalLigne);
            TxtTotal.Text = $"{total:N2} €";
        }
        public class LignePanier
        {
            // Le "required" force à remplir la variable lors de la création
            public required Piece PieceOriginale { get; set; }
            public string NomPiece { get; set; } = string.Empty;
            public int Quantite { get; set; }
            public decimal PrixUnitaire { get; set; }
            public decimal TotalLigne => Quantite * PrixUnitaire;
        }

        // Le handler de validation est défini plus haut (gère aussi l'export PDF). Ne pas dupliquer.
    }
}