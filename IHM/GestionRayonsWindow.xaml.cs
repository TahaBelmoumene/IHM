using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class GestionRayonsWindow : Window
    {
        private GarageRepository _repo;
        private Categorie _parentNiv1;
        private Categorie _parentNiv2;

        public GestionRayonsWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerNiveau1();
        }

        private void ChargerNiveau1()
        {
            LstNiveau1.ItemsSource = _repo.GetRayonsPrincipaux();
        }

        private void BtnAjoutNiv1_Click(object sender, RoutedEventArgs e)
        {
            string nom = TxtNiveau1.Text;
            if (!string.IsNullOrWhiteSpace(nom))
            {
                _repo.AjouterCategorie(nom, null);
                TxtNiveau1.Text = "";
                ChargerNiveau1();

                foreach (Categorie c in LstNiveau1.Items)
                {
                    if (c.Nom == nom)
                    {
                        LstNiveau1.SelectedItem = c;
                        break;
                    }
                }
            }
        }

        private void LstNiveau1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstNiveau1.SelectedItem is Categorie cat)
            {
                _parentNiv1 = cat;

                GrpNiveau2.IsEnabled = true;
                GrpNiveau3.IsEnabled = false;

                ChargerNiveau2();
                LstNiveau3.ItemsSource = null;
            }
        }

        private void ChargerNiveau2()
        {
            if (_parentNiv1 != null)
                LstNiveau2.ItemsSource = _repo.GetSousCategories(_parentNiv1.Id);
        }

        private void BtnAjoutNiv2_Click(object sender, RoutedEventArgs e)
        {
            string nom = TxtNiveau2.Text;
            if (!string.IsNullOrWhiteSpace(nom) && _parentNiv1 != null)
            {
                _repo.AjouterCategorie(nom, _parentNiv1.Id);
                TxtNiveau2.Text = "";
                ChargerNiveau2();

                foreach (Categorie c in LstNiveau2.Items)
                {
                    if (c.Nom == nom)
                    {
                        LstNiveau2.SelectedItem = c;
                        break;
                    }
                }
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

        private void ChargerNiveau3()
        {
            if (_parentNiv2 != null)
                LstNiveau3.ItemsSource = _repo.GetSousCategories(_parentNiv2.Id);
        }

        private void BtnAjoutNiv3_Click(object sender, RoutedEventArgs e)
        {
            string nom = TxtNiveau3.Text;
            if (!string.IsNullOrWhiteSpace(nom) && _parentNiv2 != null)
            {
                _repo.AjouterCategorie(nom, _parentNiv2.Id);
                TxtNiveau3.Text = "";
                ChargerNiveau3();

                foreach (Categorie c in LstNiveau3.Items)
                {
                    if (c.Nom == nom)
                    {
                        LstNiveau3.SelectedItem = c;
                        break;
                    }
                }
            }
        }
    }
}