using System.Windows;
using IHM; // Ajoutez cette directive using si AjouterPieceWindow est dans le namespace IHM.Windows

namespace IHM
{
    public partial class Accueil : Window
    {
        public Accueil()
        {
            InitializeComponent();
        }

        // 1. Ouvre la recherche (Ton ancienne MainWindow)
        private void BtnTrouver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow fenetreRecherche = new MainWindow();
            fenetreRecherche.Show(); // Affiche la fenêtre de recherche
            this.Close();            // Ferme le menu principal
        }

        // 2. Placeholder pour Ajouter Pièce
        private void BtnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            AjouterPieceWindow fenetre = new AjoutPieceWindow();
            fenetre.ShowDialog();
        }

        // 3. Placeholder pour Modifier Pièce
        private void BtnModifierPiece_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Écran de modification à créer !");
        }

        // 4. Placeholder pour Ajouter Voiture
        private void BtnAjouterVoiture_Click(object sender, RoutedEventArgs e)
        {
            // C'est ici qu'on ouvre la fenêtre de création en cascade
            AjoutVoitureWindow fenetreAjout = new AjoutVoitureWindow();
            fenetreAjout.ShowDialog();
        }
    }
}