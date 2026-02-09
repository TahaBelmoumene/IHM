using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class AjoutPieceWindow : Window
    {
        private GarageRepository _repo;

        private Categorie? _categorieFinale;
        private Motorisation? _moteurSelectionne;

        public AjoutPieceWindow()
        {
            InitializeComponent(); 
            _repo = new GarageRepository();

            ChargerNiveau1(); // Charge les rayons
            ChargerMarques(); // Charge les marques
        }

      
        private void ChargerMarques()
        {
            CboMarque.ItemsSource = _repo.GetMarques();
        }

        private void CboMarque_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboMarque.SelectedItem is Marque m)
            {
                CboModele.ItemsSource = _repo.GetModeles(m.Id);
                CboModele.IsEnabled = true;
                CboGeneration.ItemsSource = null; CboGeneration.IsEnabled = false;
                CboMoteur.ItemsSource = null; CboMoteur.IsEnabled = false;
                ValiderBouton();
            }
        }

        private void CboModele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboModele.SelectedItem is Modele m)
            {
                CboGeneration.ItemsSource = _repo.GetGenerations(m.Id);
                CboGeneration.IsEnabled = true;
                CboMoteur.ItemsSource = null; CboMoteur.IsEnabled = false;
                ValiderBouton();
            }
        }

        private void CboGeneration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboGeneration.SelectedItem is Generation g)
            {
                CboMoteur.ItemsSource = _repo.GetMoteurs(g.Id);
                CboMoteur.IsEnabled = true;
                ValiderBouton();
            }
        }

        private void CboMoteur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboMoteur.SelectedItem is Motorisation m)
            {
                _moteurSelectionne = m;
                ValiderBouton();
            }
        }

       
        private void ChargerNiveau1()
        {
            CboNiv1.ItemsSource = _repo.GetRayonsPrincipaux();
        }

        private void CboNiv1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv1.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat;
                ValiderBouton();

                CboNiv2.ItemsSource = _repo.GetSousCategories(cat.Id);
                CboNiv2.IsEnabled = true;
                CboNiv3.ItemsSource = null; CboNiv3.IsEnabled = false;
            }
        }

        private void CboNiv2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv2.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat;
                ValiderBouton();

                CboNiv3.ItemsSource = _repo.GetSousCategories(cat.Id);
                CboNiv3.IsEnabled = true;
            }
        }

        private void CboNiv3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv3.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat;
            }
        }

       
        private void ValiderBouton()
        {
            bool categorieOk = _categorieFinale != null;
            bool moteurOk = _moteurSelectionne != null;

            BtnValider.IsEnabled = categorieOk && moteurOk;
            BtnValider.Opacity = (categorieOk && moteurOk) ? 1 : 0.5;
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNom.Text)) { MessageBox.Show("Il manque le nom !"); return; }
            if (!decimal.TryParse(TxtPrix.Text.Replace(".", ","), out decimal prix)) { MessageBox.Show("Prix invalide"); return; }
            if (!int.TryParse(TxtStock.Text, out int stock)) { MessageBox.Show("Stock invalide"); return; }

            if (_categorieFinale == null || _moteurSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un rayon et un véhicule complet.");
                return;
            }

            _repo.AjouterPiece(TxtNom.Text, prix, stock, "Neuf", _categorieFinale.Id, _moteurSelectionne.Id);

            MessageBox.Show($"Pièce ajoutée pour {_moteurSelectionne.Nom} !");
            this.Close();
        }
    }
}