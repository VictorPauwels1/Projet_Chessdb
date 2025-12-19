using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;

namespace Projet_Chess_db.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {

        private readonly IDataService _dataService;
        private readonly IEloCalculator _eloCalculator;


        public ObservableCollection<Player> Players { get; set; }

        // Joueur sélectionné dans l'interface
        private Player _selectedPlayer;
        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }


        public PlayerViewModel(IDataService dataService, IEloCalculator eloCalculator)
        {
            _dataService = dataService;
            _eloCalculator = eloCalculator;

            Players = new ObservableCollection<Player>();

            LoadPlayers();
        }

        // Charge les joueurs depuis le fichier
        public void LoadPlayers()
        {
            var players = _dataService.LoadPlayers();

            Players.Clear();
            foreach (var player in players)
            {
                Players.Add(player);
            }
        }

        // Ajoute un nouveau joueur
        public void AddPlayer(Player player)
        {

            player.Id = Players.Count > 0 ? Players.Max(p => p.Id) + 1 : 1;


            Players.Add(player);


            SavePlayers();
        }

        // Supprime un joueur
        public void DeletePlayer(int playerId)
        {
            var player = Players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                Players.Remove(player);
                SavePlayers();
            }
        }

        public void UpdatePlayer(Player updatedPlayer)
        {
            var existingPlayer = Players.FirstOrDefault(p => p.Id == updatedPlayer.Id);
            if (existingPlayer == null)
                return;

            // Mettre à jour les propriétés
            existingPlayer.FirstName = updatedPlayer.FirstName;
            existingPlayer.LastName = updatedPlayer.LastName;
            existingPlayer.DateOfBirth = updatedPlayer.DateOfBirth;
            existingPlayer.Email = updatedPlayer.Email;
            existingPlayer.PhoneNumber = updatedPlayer.PhoneNumber;
            existingPlayer.EloRating = updatedPlayer.EloRating;

            SavePlayers();
            OnPropertyChanged(nameof(Players));
        }
        // Met à jour l'ELO d'un joueur après une partie
        public void UpdatePlayerElo(int playerId, int newElo)
        {
            var player = Players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                player.EloRating = newElo;
                SavePlayers();
                OnPropertyChanged(nameof(Players));
            }
        }

        // Sauvegarde tous les joueurs
        private void SavePlayers()
        {
            _dataService.SavePlayers(Players.ToList());
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
