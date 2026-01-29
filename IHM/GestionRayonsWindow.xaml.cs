using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class GestionRayonsWindow : Window
    {
        private GarageRepository _repo;
        private Categorie _parentNiv1; // Le Rayon sélectionné (ex: Freinage)
        private Categorie _parentNiv2; // La Famille sélectionnée (ex: Plaquettes)

        public GestionRayonsWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerNiveau1();
        }

        // --- NIVEAU 1 : LES RAYONS (ex: Moteur, Freinage...) ---
        private void ChargerNiveau1()
        {
            LstNiveau1.ItemsSource = _repo.GetRayonsPrincipaux();
        }

        private void BtnAjoutNiv1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNiveau1.Text))
            {
                _repo.AjouterCategorie(TxtNiveau1.Text, null); // Pas de parent
                TxtNiveau1.Text = "";
                ChargerNiveau1();
            }
        }

        private void LstNiveau1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstNiveau1.SelectedItem is Categorie cat)
            {
                _parentNiv1 = cat;

                // On active la colonne suivante
                GrpNiveau2.IsEnabled = true;
                GrpNiveau3.IsEnabled = false; // On désactive la 3ème par sécurité

                ChargerNiveau2();
                LstNiveau3.ItemsSource = null; // On vide la liste 3
            }
        }

        // --- NIVEAU 2 : LES FAMILLES (ex: Disques, Plaquettes...) ---
        private void ChargerNiveau2()
        {
            if (_parentNiv1 != null)
                LstNiveau2.ItemsSource = _repo.GetSousCategories(_parentNiv1.Id);
        }

        private void BtnAjoutNiv2_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNiveau2.Text) && _parentNiv1 != null)
            {
                _repo.AjouterCategorie(TxtNiveau2.Text, _parentNiv1.Id);
                TxtNiveau2.Text = "";
                ChargerNiveau2();
            }
        }

        private void LstNiveau2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstNiveau2.SelectedItem is Categorie cat)
            {
                _parentNiv2 = cat;

                // On active la dernière colonne
                GrpNiveau3.IsEnabled = true;
                ChargerNiveau3();
            }
        }

        // --- NIVEAU 3 : LES TYPES (ex: Avant, Arrière...) ---
        private void ChargerNiveau3()
        {
            if (_parentNiv2 != null)
                LstNiveau3.ItemsSource = _repo.GetSousCategories(_parentNiv2.Id);
        }

        private void BtnAjoutNiv3_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNiveau3.Text) && _parentNiv2 != null)
            {
                _repo.AjouterCategorie(TxtNiveau3.Text, _parentNiv2.Id);
                TxtNiveau3.Text = "";
                ChargerNiveau3();
            }
        }
    }
}