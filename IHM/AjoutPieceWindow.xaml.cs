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

            ChargerNiveau1();
            ChargerOrigines();
        }

        private void ChargerOrigines()
        {
            CboOrigine.ItemsSource = _repo.GetOrigines();
        }

        private void CboOrigine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboOrigine.SelectedItem is Marque origine)
            {
                CboMarque.ItemsSource = _repo.GetMarquesParOrigine(origine.Id);
                CboMarque.IsEnabled = true;

                CboModele.ItemsSource = null; CboModele.IsEnabled = false;
                CboGeneration.ItemsSource = null; CboGeneration.IsEnabled = false;
                CboMoteur.ItemsSource = null; CboMoteur.IsEnabled = false;
                ValiderBouton();
            }
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

            // Le bouton Enregistrer ne dépend plus que du rayon (catégorie)
            BtnValider.IsEnabled = categorieOk;
            BtnValider.Opacity = categorieOk ? 1 : 0.5;

            // Le bouton Pack Démarrage dépend toujours du moteur
            BtnPack.IsEnabled = moteurOk;
            BtnPack.Opacity = moteurOk ? 1 : 0.5;
        }
        private void BtnPack_Click(object sender, RoutedEventArgs e)
        {
            if (_moteurSelectionne == null) return;

            var resultat = MessageBox.Show(
                $"Voulez-vous générer automatiquement les pièces courantes (Filtres, Freins, Batterie...) pour {_moteurSelectionne.Nom} ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultat == MessageBoxResult.Yes)
            {
                _repo.AjouterPackDemarrage(_moteurSelectionne.Id);
                MessageBox.Show("Pack de démarrage ajouté avec succès !");
                this.Close();
            }
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNom.Text)) { MessageBox.Show("Il manque le nom !"); return; }

            if (!decimal.TryParse(TxtPrix.Text.Replace(".", ","), out decimal prix)) { MessageBox.Show("Prix invalide"); return; }
            if (!int.TryParse(TxtStock.Text, out int stock)) { MessageBox.Show("Stock invalide"); return; }

            if (_categorieFinale == null)
            {
                MessageBox.Show("Veuillez sélectionner au moins un rayon pour ranger la pièce.");
                return;
            }

            int? idMoteur = null;

            // Si aucune voiture n'est sélectionnée, on demande confirmation
            if (_moteurSelectionne == null)
            {
                var resultat = MessageBox.Show(
                    "Êtes-vous sûr de vouloir ajouter cette pièce sans sélectionner de voiture (pièce universelle) ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultat == MessageBoxResult.No)
                {
                    return; // L'utilisateur a cliqué sur "Non", on annule l'enregistrement
                }
            }
            else
            {
                idMoteur = _moteurSelectionne.Id;
            }

            string etat = "Neuf";
            if (CboEtat.SelectedItem is ComboBoxItem item)
            {
                etat = item.Content.ToString();
            }

            // Appel au repo mis à jour
            _repo.AjouterPiece(TxtNom.Text, prix, stock, etat, _categorieFinale.Id, idMoteur);

            if (idMoteur != null)
                MessageBox.Show($"Pièce ajoutée pour {_moteurSelectionne.Nom} !");
            else
                MessageBox.Show("Pièce universelle ajoutée avec succès !");

            this.Close();
        }
    }
}