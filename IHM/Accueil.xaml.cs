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
            this.Hide(); // Cache l'accueil
            MainWindow fenetreRecherche = new MainWindow();
            fenetreRecherche.ShowDialog(); // Attend que la fenêtre se ferme
            this.Show(); // Réaffiche l'accueil
        }

        private void BtnAjouterPiece_Click(object sender, RoutedEventArgs e)
        {
            AjoutPieceWindow fenetre = new AjoutPieceWindow();
            fenetre.ShowDialog();
        }

        private void BtnModifierPiece_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Cache l'accueil
            ChoixCategorieWindow fenetre = new ChoixCategorieWindow(null);
            fenetre.ShowDialog(); // Attend que la fenêtre se ferme
            this.Show(); // Réaffiche l'accueil
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
        private void BtnFacture_Click(object sender, RoutedEventArgs e)
        {
            // On ouvre la fenêtre de création de facture
            CreationFactureWindow fenetre = new CreationFactureWindow();
            fenetre.ShowDialog(); // ShowDialog empêche de cliquer ailleurs tant que la facture n'est pas finie
        }
        private void BtnNouveauClient_Click(object sender, RoutedEventArgs e)
        {
            AjoutClientWindow fenetre = new AjoutClientWindow();
            fenetre.ShowDialog(); // ShowDialog empêche de cliquer ailleurs tant que c'est ouvert
        }
    }
}