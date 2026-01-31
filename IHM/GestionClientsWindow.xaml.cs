using System.Windows;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class GestionClientsWindow : Window
    {
        private GarageRepository _repo;
        public Client ClientSelectionne { get; private set; } // Pour récupérer le choix

        public GestionClientsWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerClients();
        }

        private void ChargerClients()
        {
            LstClients.ItemsSource = _repo.GetClients();
        }

        private void BtnCreerClient_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                _repo.AjouterClient(TxtNom.Text, TxtPrenom.Text, TxtTel.Text);
                TxtNom.Text = ""; TxtPrenom.Text = ""; TxtTel.Text = "";
                ChargerClients(); // On rafraîchit la liste
            }
        }

        private void BtnChoisir_Click(object sender, RoutedEventArgs e)
        {
            if (LstClients.SelectedItem is Client c)
            {
                ClientSelectionne = c;
                this.DialogResult = true; // Ferme la fenêtre en disant "OK"
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un client dans la liste.");
            }
        }
    }
}