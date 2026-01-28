using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class GestionRayonsWindow : Window
    {
        private GarageRepository _repo;
        private Categorie _parentNiv1; // Sélection Niveau 1
        private Categorie _parentNiv2; // Sélection Niveau 2

        public GestionRayonsWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerNiveau1();
        }

        // --- NIVEAU 1 ---
        private void ChargerNiveau1() => LstNiveau1.ItemsSource = _repo.GetRayonsPrincipaux();

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
                GrpNiveau2.IsEnabled = true;
                GrpNiveau3.IsEnabled = false; // Reset niveau 3
                ChargerNiveau2();
                LstNiveau3.ItemsSource = null; // Vider liste 3
            }
        }

        // --- NIVEAU 2 ---
        private void ChargerNiveau2() => LstNiveau2.ItemsSource = _repo.GetSousCategories(_parentNiv1.Id);

        private void BtnAjoutNiv2_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNiveau2.Text))
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
                GrpNiveau3.IsEnabled = true;
                ChargerNiveau3();
            }
        }

        // --- NIVEAU 3 ---
        private void ChargerNiveau3() => LstNiveau3.ItemsSource = _repo.GetSousCategories(_parentNiv2.Id);

        private void BtnAjoutNiv3_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNiveau3.Text))
            {
                _repo.AjouterCategorie(TxtNiveau3.Text, _parentNiv2.Id);
                TxtNiveau3.Text = "";
                ChargerNiveau3();
            }
        }
    }
}