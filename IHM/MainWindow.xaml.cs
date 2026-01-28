using System.Windows;
using IHM.ViewModels; // Important pour trouver MainViewModel

namespace IHM
{
    // Cette classe "partial" est OBLIGATOIRE et doit s'appeler MainWindow
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // L'erreur disparaîtra ici

            // On lie la fenêtre à son ViewModel
            this.DataContext = new MainViewModel();
        }
    }
}