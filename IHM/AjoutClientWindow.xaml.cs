using System.Windows;
using Metier.Data;

namespace IHM
{
    public partial class AjoutClientWindow : Window
    {
        private GarageRepository _repo;

        public AjoutClientWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                MessageBox.Show("Le nom est obligatoire.");
                return;
            }

            // Appel de la méthode existante dans ton repo
            _repo.AjouterClient(TxtNom.Text, TxtPrenom.Text, TxtTel.Text);

            MessageBox.Show("Client enregistré avec succès !");
            this.Close();
        }
    }
}