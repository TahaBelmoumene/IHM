using System.Windows;
using IHM.ViewModels; // Pour accéder au ViewModel

namespace IHM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // On connecte la fenêtre aux données (ViewModel)
            this.DataContext = new MainViewModel();
        }

        // C'est cette méthode qui manquait !
        private void BtnVoirPieces_Click(object sender, RoutedEventArgs e)
        {
            // 1. On récupère les infos de l'écran (le ViewModel)
            var vm = (MainViewModel)this.DataContext;

            // 2. On vérifie si un moteur est bien sélectionné
            if (vm.MoteurSelected != null)
            {
                // 3. On ouvre la fenêtre des catégories (Freinage, Moteur...) 
                // en lui envoyant la voiture choisie.
                ChoixCategorieWindow fenetreCat = new ChoixCategorieWindow(vm.MoteurSelected);
                fenetreCat.Show();

                // 4. On ferme la recherche car on a trouvé le véhicule
                this.Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un véhicule complet (jusqu'au moteur) avant de continuer !");
            }
        }
    }
}