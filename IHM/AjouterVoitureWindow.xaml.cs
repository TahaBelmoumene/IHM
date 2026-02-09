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

            string nomMarque = TxtMarque.Text;
            if (!string.IsNullOrWhiteSpace(nomMarque))
            {
                _repo.AjouterMarque(nomMarque, _origineEnCours.Id);

                TxtMarque.Text = "";
                ChargerMarques();

                foreach (Marque m in CboMarques.Items)
                {
                    if (m.Nom == nomMarque)
                    {
                        CboMarques.SelectedItem = m;
                        break;
                    }
                }
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
            string nomModele = TxtModele.Text;
            if (!string.IsNullOrWhiteSpace(nomModele) && _marqueEnCours != null)
            {
                _repo.AjouterModele(nomModele, _marqueEnCours.Id);
                TxtModele.Text = "";
                ChargerModeles();

                foreach (Modele m in CboModeles.Items)
                {
                    if (m.Nom == nomModele)
                    {
                        CboModeles.SelectedItem = m;
                        break;
                    }
                }
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
            string nomGen = TxtGenNom.Text;
            if (!string.IsNullOrWhiteSpace(nomGen) && int.TryParse(TxtGenAnnee.Text, out int annee))
            {
                _repo.AjouterGeneration(nomGen, annee, _modeleEnCours.Id);
                TxtGenNom.Text = ""; TxtGenAnnee.Text = "";
                ChargerGenerations();

                foreach (Generation g in CboGenerations.Items)
                {
                    if (g.Nom == nomGen)
                    {
                        CboGenerations.SelectedItem = g;
                        break;
                    }
                }
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