using System; // Important pour Action<>
using System.Windows;
using System.Windows.Input;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ChoixCategorieWindow : Window
    {
        private GarageRepository _repo;
        private Motorisation? _voitureChoisie;
        private Action<Piece>? _callbackSelection; // Le passe-plat

        // Ajout du paramètre callback
        public ChoixCategorieWindow(Motorisation? voiture = null, Action<Piece>? onPieceSelected = null)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _voitureChoisie = voiture;
            _callbackSelection = onPieceSelected; // On stocke l'action

            if (_callbackSelection != null)
                TxtTitreVoiture.Text = "Sélectionnez une catégorie pour la facture";
            else if (_voitureChoisie != null)
                TxtTitreVoiture.Text = $"Recherche pour : {_voitureChoisie.Nom}";
            else
                TxtTitreVoiture.Text = "Mode Inventaire Global";

            ChargerNiveau(null);
        }

        private void ChargerNiveau(int? parentId)
        {
            var liste = (parentId == null) ? _repo.GetRayonsPrincipaux() : _repo.GetSousCategories(parentId.Value);
            LstCategories.ItemsSource = liste;
        }

        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie selection)
            {
                var enfants = _repo.GetSousCategories(selection.Id);

                if (enfants.Count > 0)
                {
                    TxtTitreVoiture.Text = $"Rayon : {selection.Nom}";
                    LstCategories.ItemsSource = enfants;
                }
                else
                {
                    // C'est ici qu'on passe le _callbackSelection à la fenêtre suivante
                    ListePiecesWindow fenetre = new ListePiecesWindow(selection, _voitureChoisie, _callbackSelection);

                    if (_callbackSelection != null)
                    {
                        // Si on est en mode sélection, on ferme cette fenêtre de catégorie une fois la liste ouverte
                        // ou on la laisse ouverte, mais fermons-la pour faire propre
                        fenetre.Show(); // On affiche la suivante
                        this.Close();   // On ferme celle-ci
                    }
                    else
                    {
                        fenetre.ShowDialog();
                    }
                }
            }
        }
    }
}