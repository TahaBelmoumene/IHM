using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class AjoutVoitureWindow : Window
    {
        private GarageRepository _repo;

        // On stocke les sélections en cours
        private Marque _marqueEnCours;
        private Modele _modeleEnCours;
        private Generation _genEnCours;

        public AjoutVoitureWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerMarques();
        }

        // --- ZONE 1 : MARQUE ---
        private void ChargerMarques()
        {
            CboMarques.ItemsSource = _repo.GetMarques();
        }

        private void BtnAjoutMarque_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtMarque.Text))
            {
                _repo.AjouterMarque(TxtMarque.Text);
                TxtMarque.Text = "";
                ChargerMarques(); // Rafraîchir la liste
                MessageBox.Show("Marque ajoutée ! Sélectionnez-la dans la liste.");
            }
        }

        private void CboMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboMarques.SelectedItem is Marque m)
            {
                _marqueEnCours = m;
                GrpModele.IsEnabled = true; // On débloque l'étape suivante
                ChargerModeles();

                // On reset les étapes d'après pour éviter les mélanges
                GrpGeneration.IsEnabled = false;
                GrpMoteur.IsEnabled = false;
            }
        }

        // --- ZONE 2 : MODÈLE ---
        private void ChargerModeles()
        {
            if (_marqueEnCours != null)
                CboModeles.ItemsSource = _repo.GetModeles(_marqueEnCours.Id);
        }

        private void BtnAjoutModele_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtModele.Text) && _marqueEnCours != null)
            {
                _repo.AjouterModele(TxtModele.Text, _marqueEnCours.Id);
                TxtModele.Text = "";
                ChargerModeles();
                MessageBox.Show($"Modèle ajouté chez {_marqueEnCours.Nom} !");
            }
        }

        private void CboModeles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboModeles.SelectedItem is Modele m)
            {
                _modeleEnCours = m;
                GrpGeneration.IsEnabled = true; // On débloque l'étape suivante
                ChargerGenerations();
                GrpMoteur.IsEnabled = false;
            }
        }

        // --- ZONE 3 : GÉNÉRATION ---
        private void ChargerGenerations()
        {
            if (_modeleEnCours != null)
                CboGenerations.ItemsSource = _repo.GetGenerations(_modeleEnCours.Id);
        }

        private void BtnAjoutGen_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtGenNom.Text) && int.TryParse(TxtGenAnnee.Text, out int annee))
            {
                _repo.AjouterGeneration(TxtGenNom.Text, annee, _modeleEnCours.Id);
                TxtGenNom.Text = ""; TxtGenAnnee.Text = "";
                ChargerGenerations();
                MessageBox.Show("Version ajoutée !");
            }
            else
            {
                MessageBox.Show("Vérifiez le nom et l'année.");
            }
        }

        private void CboGenerations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboGenerations.SelectedItem is Generation g)
            {
                _genEnCours = g;
                GrpMoteur.IsEnabled = true; // On débloque la dernière étape
            }
        }

        // --- ZONE 4 : MOTEUR ---
        private void BtnAjoutMoteur_Click(object sender, RoutedEventArgs e)
        {
            string nomMoteur = TxtMoteurNom.Text;
            string carburant = (CboCarburant.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrWhiteSpace(nomMoteur) && _genEnCours != null)
            {
                _repo.AjouterMoteur(nomMoteur, carburant, _genEnCours.Id);
                MessageBox.Show($"Moteur {nomMoteur} ajouté à {_genEnCours.Nom} !");
                TxtMoteurNom.Text = "";
            }
        }
    }
}