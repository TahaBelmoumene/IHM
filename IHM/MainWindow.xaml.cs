using System.Windows;
using IHM.ViewModels; 
namespace IHM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }

        private void BtnVoirPieces_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)this.DataContext;

            if (vm.MoteurSelected != null)
            {
                this.Hide(); // Cache la fenêtre de recherche
                ChoixCategorieWindow fenetreCat = new ChoixCategorieWindow(vm.MoteurSelected);
                fenetreCat.ShowDialog();
                this.Close(); // Quand on a fini avec les catégories, on ferme celle-ci pour retourner à l'accueil
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un véhicule complet (jusqu'au moteur) avant de continuer !");
            }
        }
    }
}