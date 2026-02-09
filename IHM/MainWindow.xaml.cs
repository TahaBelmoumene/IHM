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
                
                ChoixCategorieWindow fenetreCat = new ChoixCategorieWindow(vm.MoteurSelected);
                fenetreCat.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un véhicule complet (jusqu'au moteur) avant de continuer !");
            }
        }
    }
}