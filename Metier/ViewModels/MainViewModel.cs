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

        private Marque _origineSelected;
        public Marque OrigineSelected
        {
            get => _origineSelected;
            set
            {
                _origineSelected = value;
                OnPropertyChanged();
                ChargerMarques();
            }
        }

        private Marque _marqueSelected;
        public Marque MarqueSelected
        {
            get => _marqueSelected;
            set
            {
                _marqueSelected = value;
                OnPropertyChanged();
                ChargerModeles();
            }
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
            _repo = new GarageRepository();
            // Chargement initial des Origines (Italienne, Française, Allemande)
            foreach (var o in _repo.GetOrigines())
                ListeOrigines.Add(o);
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}