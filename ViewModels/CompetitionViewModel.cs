using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;

namespace Projet_Chess_db.ViewModels
{
    public class CompetitionViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;

        public ObservableCollection<Competition> Competitions { get; set; }
        public ObservableCollection<Player> Players { get; set; }

        private Competition _selectedCompetition;
        public Competition SelectedCompetition
        {
            get => _selectedCompetition;
            set
            {
                _selectedCompetition = value;
                OnPropertyChanged(nameof(SelectedCompetition));
            }
        }

        public CompetitionViewModel(IDataService dataService)
        {
            _dataService = dataService;

            Competitions = new ObservableCollection<Competition>();
            Players = new ObservableCollection<Player>();

            LoadData();
        }

        private void LoadData()
        {

            var competitions = _dataService.LoadCompetitions();
            Competitions.Clear();
            foreach (var comp in competitions)
            {
                Competitions.Add(comp);
            }


            var players = _dataService.LoadPlayers();
            Players.Clear();
            foreach (var player in players)
            {
                Players.Add(player);
            }
        }

        // Ajoute une nouvelle compétition
        public void AddCompetition(Competition competition)
        {
            competition.Id = Competitions.Count > 0 ? Competitions.Max(c => c.Id) + 1 : 1;
            Competitions.Add(competition);
            SaveCompetitions();
        }

        // Inscrit un joueur à une compétition
        public bool RegisterPlayer(int competitionId, int playerId)
        {
            var competition = Competitions.FirstOrDefault(c => c.Id == competitionId);
            if (competition == null)
                return false;

            bool success = competition.RegisterPlayer(playerId);
            if (success)
            {
                SaveCompetitions();
                OnPropertyChanged(nameof(SelectedCompetition));
            }
            return success;
        }

        // Désinscrit un joueur
        public bool UnregisterPlayer(int competitionId, int playerId)
        {
            var competition = Competitions.FirstOrDefault(c => c.Id == competitionId);
            if (competition == null)
                return false;

            bool success = competition.UnregisterPlayer(playerId);
            if (success)
            {
                SaveCompetitions();
                OnPropertyChanged(nameof(SelectedCompetition));
            }
            return success;
        }

        private void SaveCompetitions()
        {
            _dataService.SaveCompetitions(Competitions.ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}