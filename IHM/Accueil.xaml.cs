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