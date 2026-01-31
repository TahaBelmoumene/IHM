using IHM.ViewModels; // Pour accéder au ViewModel
using Metier.Data;
using System.Windows;

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
        private void BtnChercherPlaque_Click(object sender, RoutedEventArgs e)
        {
            string plaque = TxtPlaque.Text.Trim();
            if (string.IsNullOrWhiteSpace(plaque)) return;

            var vm = (MainViewModel)this.DataContext;
            GarageRepository repo = new GarageRepository();

            // On cherche dans NOTRE base de données
            var moteurTrouve = repo.GetVoitureParPlaque(plaque);

            if (moteurTrouve != null)
            {
                // MAGIE : On force la sélection dans le ViewModel
                // Il faut remonter la chaîne : Marque -> Modèle -> Génération -> Moteur

                var gen = moteurTrouve.Generation;
                var mod = gen.Modele;
                var mar = mod.Marque;

                // On sélectionne la Marque (ça déclenchera le chargement des modèles)
                vm.MarqueSelected = vm.ListeMarques.FirstOrDefault(m => m.Id == mar.Id);

                // On sélectionne le Modèle
                vm.ModeleSelected = vm.ListeModeles.FirstOrDefault(m => m.Id == mod.Id);

                // On sélectionne la Génération
                vm.GenerationSelected = vm.ListeGenerations.FirstOrDefault(g => g.Id == gen.Id);

                // On sélectionne le Moteur
                vm.MoteurSelected = vm.ListeMoteurs.FirstOrDefault(m => m.Id == moteurTrouve.Id);

                MessageBox.Show($"Véhicule trouvé : {mar.Nom} {mod.Nom} !");
            }
            else
            {
                MessageBox.Show("Plaque inconnue dans votre garage. Sélectionnez le véhicule manuellement puis cliquez sur 'Mémoriser'.");
            }
        }

        // 2. ENREGISTRER UNE NOUVELLE PLAQUE
        private void BtnMemoriserPlaque_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)this.DataContext;
            string plaque = TxtPlaque.Text.Trim();

            if (string.IsNullOrWhiteSpace(plaque)) { MessageBox.Show("Entrez une plaque !"); return; }
            if (vm.MoteurSelected == null) { MessageBox.Show("Sélectionnez d'abord un véhicule complet."); return; }

            GarageRepository repo = new GarageRepository();
            repo.EnregistrerPlaque(plaque, vm.MoteurSelected.Id);

            MessageBox.Show("✅ Plaque mémorisée ! La prochaine fois, la recherche sera automatique.");
        }
        private void BtnVoirPieces_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)this.DataContext;

            if (vm.MoteurSelected != null)
            {
                ChoixCategorieWindow fenetreCat = new ChoixCategorieWindow(vm.MoteurSelected);

                // ASTUCE : Quand on aura fini avec les catégories, on reviendra sur la recherche
                fenetreCat.Closed += (s, args) => this.Show();

                fenetreCat.Show();
                this.Hide(); // On cache la recherche pour laisser place aux catégories
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un véhicule complet.");
            }
        }
    }
}