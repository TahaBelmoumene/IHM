using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;
using Microsoft.Win32; // Pour SaveFileDialog
using IHM.Services;    // Pour FactureExporter

namespace IHM
{
    // Classe locale pour l'affichage dans le Panier
    public class LignePanier
    {
        public Piece PieceOriginale { get; set; } = null!;
        public string NomPiece { get; set; } = "";
        public string Etat { get; set; } = "Neuf"; // Info ajoutée pour la TVA
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }

        public decimal TotalLigne => Quantite * PrixUnitaire;

        // Propriété combinée pour l'affichage (ex: "Alternateur (Occasion)")
        public string DescriptionComplete => $"{NomPiece} ({Etat})";
    }

    public partial class CreationFactureWindow : Window
    {
        private GarageRepository _repo;
        private List<LignePanier> _panier;

        // Stocke la pièce que l'utilisateur vient de choisir via la fenêtre de recherche
        private Piece? _pieceEnCours;

        public CreationFactureWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _panier = new List<LignePanier>();

            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            // On ne charge plus que les clients. Les pièces sont chargées via la recherche.
            CboClients.ItemsSource = _repo.GetClients();
        }

        // --- NOUVELLE MÉTHODE : Ouvre la fenêtre de recherche ---
        private void BtnRechercherPiece_Click(object sender, RoutedEventArgs e)
        {
            // On ouvre la fenêtre de catégories en lui passant une "Action" (ce code s'exécutera quand on choisira une pièce)
            ChoixCategorieWindow fenetre = new ChoixCategorieWindow(null, (pieceChoisie) =>
            {
                _pieceEnCours = pieceChoisie;

                // Mise à jour visuelle de la zone de sélection
                TxtPieceSelectionnee.Text = $"{pieceChoisie.Nom} ({pieceChoisie.Prix} €)";
                TxtPieceSelectionnee.FontStyle = FontStyles.Normal;
                TxtPieceSelectionnee.FontWeight = FontWeights.Bold;

                // Pré-sélection intelligente de l'état
                if (pieceChoisie.Etat == "Occasion")
                    RadioOccasion.IsChecked = true;
                else
                    RadioNeuf.IsChecked = true;
            });

            fenetre.ShowDialog();
        }

        // --- MODIFIÉ : Ajoute la pièce sélectionnée au panier ---
        private void BtnAjouterLigne_Click(object sender, RoutedEventArgs e)
        {
            // On vérifie _pieceEnCours au lieu de CboPieces.SelectedItem
            if (_pieceEnCours != null && int.TryParse(TxtQte.Text, out int qte) && qte > 0)
            {
                // Vérifier le stock
                if (_pieceEnCours.Stock < qte)
                {
                    MessageBox.Show($"Stock insuffisant ! (Dispo : {_pieceEnCours.Stock})");
                    return;
                }

                // Récupérer l'état choisi (Neuf ou Occasion)
                string etatChoisi = (RadioOccasion.IsChecked == true) ? "Occasion" : "Neuf";

                // Ajouter au panier
                _panier.Add(new LignePanier
                {
                    PieceOriginale = _pieceEnCours,
                    NomPiece = _pieceEnCours.Nom,
                    Etat = etatChoisi,
                    PrixUnitaire = _pieceEnCours.Prix,
                    Quantite = qte
                });

                RafraichirPanier();
                ResetSelection();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une pièce (via Rechercher) et une quantité valide.");
            }
        }

        private void ResetSelection()
        {
            _pieceEnCours = null;
            TxtPieceSelectionnee.Text = "Aucune pièce sélectionnée";
            TxtPieceSelectionnee.FontStyle = FontStyles.Italic;
            TxtPieceSelectionnee.FontWeight = FontWeights.Normal;
            TxtQte.Text = "1";
            RadioNeuf.IsChecked = true;
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

                // Préparation des données pour la BDD
                // On modifie le nom temporairement pour inclure l'état dans la facture finale
                var articles = _panier.Select(l =>
                {
                    var pieceTemp = l.PieceOriginale;
                    pieceTemp.Nom = $"{l.NomPiece} [{l.Etat}]"; // Astuce pour afficher l'état sur le PDF
                    return (pieceTemp, l.Quantite);
                }).ToList();

                // Création en base
                var nouvelleFacture = _repo.CreerFacture(client, articles);

                // Génération PDF
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

                        // Ouvrir le PDF
                        var p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true };
                        p.Start();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Erreur PDF : {ex.Message}");
                    }
                }

                MessageBox.Show("Facture enregistrée !");
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
    }
}