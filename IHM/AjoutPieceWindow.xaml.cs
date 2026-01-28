using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class AjoutPieceWindow : Window
    {
        private GarageRepository _repo;
        private Categorie _categorieFinale; // C'est celle qu'on va sauvegarder

        public AjoutPieceWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerNiveau1();
        }

        // --- GESTION DES CASCADES (Pareil que pour les voitures) ---

        private void ChargerNiveau1()
        {
            CboNiv1.ItemsSource = _repo.GetRayonsPrincipaux();
        }

        private void CboNiv1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv1.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat; // Pour l'instant, c'est elle la catégorie choisie
                ValiderBouton();

                // On charge le niveau suivant
                CboNiv2.ItemsSource = _repo.GetSousCategories(cat.Id);
                CboNiv2.IsEnabled = true;

                // On reset le niveau 3
                CboNiv3.ItemsSource = null;
                CboNiv3.IsEnabled = false;
            }
        }

        private void CboNiv2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv2.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat; // On précise : c'est maintenant la sous-catégorie
                ValiderBouton();

                CboNiv3.ItemsSource = _repo.GetSousCategories(cat.Id);
                CboNiv3.IsEnabled = true;
            }
        }

        private void CboNiv3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboNiv3.SelectedItem is Categorie cat)
            {
                _categorieFinale = cat; // On est au plus précis
            }
        }

        // Active le bouton seulement si une catégorie est choisie
        private void ValiderBouton()
        {
            BtnValider.IsEnabled = true;
            BtnValider.Opacity = 1;
        }

        // --- ENREGISTREMENT ---
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // 1. Vérifs
            if (string.IsNullOrWhiteSpace(TxtNom.Text)) { MessageBox.Show("Il manque le nom !"); return; }
            if (!decimal.TryParse(TxtPrix.Text.Replace(".", ","), out decimal prix)) { MessageBox.Show("Prix invalide"); return; }
            if (!int.TryParse(TxtStock.Text, out int stock)) { MessageBox.Show("Stock invalide"); return; }

            // 2. Sauvegarde
            _repo.AjouterPiece(TxtNom.Text, prix, stock, _categorieFinale.Id);

            MessageBox.Show($"✅ Pièce ajoutée dans le rayon {_categorieFinale.Nom} !");
            this.Close();
        }
    }
}