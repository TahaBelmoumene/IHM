using System.Windows;
using System.Windows.Input;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class ChoixCategorieWindow : Window
    {
        private GarageRepository _repo;
        private Motorisation _voitureChoisie; // On garde en mémoire la voiture du client

        // Constructeur qui demande quelle voiture a été choisie
        public ChoixCategorieWindow(Motorisation voiture)
        {
            InitializeComponent();
            _repo = new GarageRepository();
            _voitureChoisie = voiture;

            // On met à jour le titre
            TxtTitreVoiture.Text = $"Pièces pour : {voiture.Nom}";

            // On charge les catégories
            LstCategories.ItemsSource = _repo.GetCategories();
        }

        // Quand on double-clique sur une catégorie
        // Quand on double-clique sur une catégorie
        private void LstCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstCategories.SelectedItem is Categorie categorieSelectionnee)
            {
                // Au lieu d'un simple message, on ouvre la vraie fenêtre !
                // On passe la catégorie cliquée ET la voiture choisie au début
                ListePiecesWindow fenetre = new ListePiecesWindow(categorieSelectionnee, _voitureChoisie);
                fenetre.ShowDialog();
            }
        }
    }
}