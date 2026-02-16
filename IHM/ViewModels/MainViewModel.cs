using Metier.Data;
using Metier.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IHM.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private GarageRepository _repo;

        public ObservableCollection<Marque> ListeOrigines { get; set; } = new();
        public ObservableCollection<Marque> ListeMarques { get; set; } = new();
        public ObservableCollection<Modele> ListeModeles { get; set; } = new();
        public ObservableCollection<Generation> ListeGenerations { get; set; } = new();
        public ObservableCollection<Motorisation> ListeMoteurs { get; set; } = new();

        // 1. ORIGINE
        private Marque? _origineSelected;
        public Marque? OrigineSelected
        {
            get => _origineSelected;
            set
            {
                _origineSelected = value;
                OnPropertyChanged();

                // Quand on change l'origine, on dit à la vue de mettre à jour l'activation de la marque
                OnPropertyChanged(nameof(EstMarqueActive));

                ChargerMarques();
            }
        }

        // Propriété booléenne pour remplacer le Converter
        public bool EstMarqueActive => OrigineSelected != null;


        // 2. MARQUE
        private Marque? _marqueSelected;
        public Marque? MarqueSelected
        {
            get => _marqueSelected;
            set
            {
                _marqueSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstModeleActif)); // Mise à jour du suivant
                ChargerModeles();
            }
        }

        public bool EstModeleActif => MarqueSelected != null;


        // 3. MODELE
        private Modele? _modeleSelected;
        public Modele? ModeleSelected
        {
            get => _modeleSelected;
            set
            {
                _modeleSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstGenerationActive));
                ChargerGenerations();
            }
        }

        public bool EstGenerationActive => ModeleSelected != null;


        // 4. GENERATION (Année)
        private Generation? _genSelected;
        public Generation? GenerationSelected
        {
            get => _genSelected;
            set
            {
                _genSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstMoteurActif));
                ChargerMoteurs();
            }
        }

        public bool EstMoteurActif => GenerationSelected != null;


        // 5. MOTEUR
        private Motorisation? _moteurSelected;
        public Motorisation? MoteurSelected
        {
            get => _moteurSelected;
            set
            {
                _moteurSelected = value;
                OnPropertyChanged();
            }
        }


        // CONSTRUCTEUR
        public MainViewModel()
        {
            _repo = new GarageRepository();
            // On charge les origines (Française, Allemande...)
            foreach (var o in _repo.GetOrigines())
                ListeOrigines.Add(o);
        }

        // METHODES DE CHARGEMENT
        private void ChargerMarques()
        {
            ListeMarques.Clear(); ListeModeles.Clear(); ListeGenerations.Clear(); ListeMoteurs.Clear();
            if (OrigineSelected != null)
                foreach (var m in _repo.GetMarquesParOrigine(OrigineSelected.Id))
                    ListeMarques.Add(m);
        }

        private void ChargerModeles()
        {
            ListeModeles.Clear(); ListeGenerations.Clear(); ListeMoteurs.Clear();
            if (MarqueSelected != null)
                foreach (var m in _repo.GetModeles(MarqueSelected.Id))
                    ListeModeles.Add(m);
        }

        private void ChargerGenerations()
        {
            ListeGenerations.Clear(); ListeMoteurs.Clear();
            if (ModeleSelected != null)
                foreach (var g in _repo.GetGenerations(ModeleSelected.Id))
                    ListeGenerations.Add(g);
        }

        private void ChargerMoteurs()
        {
            ListeMoteurs.Clear();
            if (GenerationSelected != null)
                foreach (var m in _repo.GetMoteurs(GenerationSelected.Id))
                    ListeMoteurs.Add(m);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}