using System;
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

        // Nouvelle variable pour stocker l'action de retour
        private Action<Piece>? _onPieceSelected;

        // On ajoute le paramètre onPieceSelected à la fin
        public ListePiecesWindow(Categorie categorie, Motorisation? voiture = null, Action<Piece>? onPieceSelected = null)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _categorieEnCours = categorie;
            _voitureEnCours = voiture;
            _onPieceSelected = onPieceSelected;

            // Si on est en mode sélection, on change le titre
            if (_onPieceSelected != null)
            {
                TxtTitre.Text += " (Mode Sélection)";
            }

            ChargerPieces();
        }

        private void ChargerPieces()
        {
            TxtTitre.Text = _voitureEnCours != null
                ? $"{_categorieEnCours.Nom} (pour {_voitureEnCours.Nom})"
                : $"{_categorieEnCours.Nom}";

            if (_voitureEnCours == null)
                GridPieces.ItemsSource = _repo.GetPiecesParCategorie(_categorieEnCours.Id);
            else
                GridPieces.ItemsSource = _repo.GetPiecesCompatibles(_categorieEnCours.Id, _voitureEnCours.Id);
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Piece pieceSelectionnee)
            {
                // Si on a une action de sélection définie (on vient de la facture)
                if (_onPieceSelected != null)
                {
                    _onPieceSelected(pieceSelectionnee); // On renvoie la pièce
                    this.Close(); // On ferme la liste
                }
                else
                {
                    // Comportement normal (Modification)
                    ModifierPieceWindow fenetre = new ModifierPieceWindow(pieceSelectionnee);
                    fenetre.ShowDialog();
                    _repo = new GarageRepository();
                    ChargerPieces();
                }
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}