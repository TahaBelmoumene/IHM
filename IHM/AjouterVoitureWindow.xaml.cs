using System.Windows;
using System.Windows.Controls;
using Metier.Data;
using Metier.Entities;

namespace IHM
{
    public partial class AjoutVoitureWindow : Window
    {
        private GarageRepository _repo;

        private Marque _origineEnCours;
        private Marque _marqueEnCours;
        private Modele _modeleEnCours;
        private Generation _genEnCours;

        public AjoutVoitureWindow()
        {
            InitializeComponent();
            _repo = new GarageRepository();
            ChargerOrigines();
        }

        private void ChargerOrigines()
        {
            CboOrigines.ItemsSource = _repo.GetOrigines();
        }

        private void CboOrigines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboOrigines.SelectedItem is Marque origine)
            {
                _origineEnCours = origine;

                CboMarques.IsEnabled = true;
                ChargerMarques();

                GrpModele.IsEnabled = false;
                GrpGeneration.IsEnabled = false;
                GrpMoteur.IsEnabled = false;
            }
        }

        private void ChargerMarques()
        {
            if (_origineEnCours != null)
                CboMarques.ItemsSource = _repo.GetMarquesParOrigine(_origineEnCours.Id);
        }

        private void BtnAjoutMarque_Click(object sender, RoutedEventArgs e)
        {
            if (_origineEnCours == null)
            {
                MessageBox.Show("Veuillez d'abord sélectionner une Origine !");
                return;
            }

            if (!string.IsNullOrWhiteSpace(TxtMarque.Text))
            {
                _repo.AjouterMarque(TxtMarque.Text, _origineEnCours.Id);

                TxtMarque.Text = "";
                ChargerMarques();
                MessageBox.Show($"Marque ajoutée dans la catégorie {_origineEnCours.Nom} !");
            }
        }

        private void CboMarques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboMarques.SelectedItem is Marque m)
            {
                _marqueEnCours = m;
                GrpModele.IsEnabled = true;
                ChargerModeles();

                GrpGeneration.IsEnabled = false;
                GrpMoteur.IsEnabled = false;
            }
        }

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
                GrpGeneration.IsEnabled = true;
                ChargerGenerations();
                GrpMoteur.IsEnabled = false;
            }
        }

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
                GrpMoteur.IsEnabled = true;
            }
        }

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