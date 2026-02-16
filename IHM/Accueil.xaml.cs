using System.Windows;

namespace IHM
{
    public partial class Accueil : Window
    {
        public Accueil()
        {
            InitializeComponent();
        }

        private void BtnTrouver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow fenetreRecherche = new MainWindow();
            fenetreRecherche.Show();
            this.Close();
        }

        private void BtnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            AjoutPieceWindow fenetre = new AjoutPieceWindow();
            fenetre.ShowDialog();
        }

        private void BtnModifierPiece_Click(object sender, RoutedEventArgs e)
        {
            // Ouvre le choix de catégorie en mode "Inventaire global" (paramètre null)
            ChoixCategorieWindow fenetre = new ChoixCategorieWindow(null);
            fenetre.Show();
        }

        private void BtnAjouterVoiture_Click(object sender, RoutedEventArgs e)
        {
            AjoutVoitureWindow fenetreAjout = new AjoutVoitureWindow();
            fenetreAjout.ShowDialog();
        }

        private void BtnGererRayons_Click(object sender, RoutedEventArgs e)
        {
            GestionRayonsWindow fenetre = new GestionRayonsWindow();
            fenetre.ShowDialog();
        }
    }
}