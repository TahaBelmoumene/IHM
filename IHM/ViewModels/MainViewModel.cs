using Metier.Data;
using Metier.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IHM.ViewModels // J'ai rangé ça proprement
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private GarageRepository _repo;

        // --- Tes listes ---
        public ObservableCollection<Marque> ListeMarques { get; set; } = new();
        public ObservableCollection<Modele> ListeModeles { get; set; } = new();
        public ObservableCollection<Generation> ListeGenerations { get; set; } = new();
        public ObservableCollection<Motorisation> ListeMoteurs { get; set; } = new();

        // --- Tes sélections ---
        private Marque _marqueSelected;
        public Marque MarqueSelected
        {
            get => _marqueSelected;
            set { _marqueSelected = value; OnPropertyChanged(); ChargerModeles(); }
        }

        private Modele _modeleSelected;
        public Modele ModeleSelected
        {
            get => _modeleSelected;
            set { _modeleSelected = value; OnPropertyChanged(); ChargerGenerations(); }
        }

        private Generation _genSelected;
        public Generation GenerationSelected
        {
            get => _genSelected;
            set { _genSelected = value; OnPropertyChanged(); ChargerMoteurs(); }
        }

        private Motorisation _moteurSelected;
        public Motorisation MoteurSelected
        {
            get => _moteurSelected;
            set { _moteurSelected = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            // IMPORTANT : Assure-toi que GarageRepository est bien "public" dans le projet Metier
            _repo = new GarageRepository();
            var marques = _repo.GetMarques();
            foreach (var m in marques) ListeMarques.Add(m);
        }

        private void ChargerModeles()
        {
            ListeModeles.Clear(); ListeGenerations.Clear(); ListeMoteurs.Clear();
            if (MarqueSelected != null)
                foreach (var m in _repo.GetModeles(MarqueSelected.Id)) ListeModeles.Add(m);
        }

        private void ChargerGenerations()
        {
            ListeGenerations.Clear(); ListeMoteurs.Clear();
            if (ModeleSelected != null)
                foreach (var g in _repo.GetGenerations(ModeleSelected.Id)) ListeGenerations.Add(g);
        }

        private void ChargerMoteurs()
        {
            ListeMoteurs.Clear();
            if (GenerationSelected != null)
                foreach (var m in _repo.GetMoteurs(GenerationSelected.Id)) ListeMoteurs.Add(m);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}