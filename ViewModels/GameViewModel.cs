using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;

namespace Projet_Chess_db.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private readonly IEloCalculator _eloCalculator;

        public ObservableCollection<Player> Players { get; set; }
        public ObservableCollection<Competition> Competitions { get; set; }

        // Partie en cours d'encodage
        private Game _currentGame;
        public Game CurrentGame
        {
            get => _currentGame;
            set
            {
                _currentGame = value;
                OnPropertyChanged(nameof(CurrentGame));
            }
        }


        private string _newMoveNotation;
        public string NewMoveNotation
        {
            get => _newMoveNotation;
            set
            {
                _newMoveNotation = value;
                OnPropertyChanged(nameof(NewMoveNotation));
            }
        }

        public GameViewModel(IDataService dataService, IEloCalculator eloCalculator)
        {
            _dataService = dataService;
            _eloCalculator = eloCalculator;

            Players = new ObservableCollection<Player>();
            Competitions = new ObservableCollection<Competition>();

            LoadData();
        }

        private void LoadData()
        {
            // Charger les joueurs
            var players = _dataService.LoadPlayers();
            Players.Clear();
            foreach (var player in players)
            {
                Players.Add(player);
            }

            // Charger les compétitions
            var competitions = _dataService.LoadCompetitions();
            Competitions.Clear();
            foreach (var comp in competitions)
            {
                Competitions.Add(comp);
            }
        }

        // Démarre une nouvelle partie
        public void StartNewGame(int competitionId, int whitePlayerId, int blackPlayerId)
        {
            var whitePlayer = Players.FirstOrDefault(p => p.Id == whitePlayerId);
            var blackPlayer = Players.FirstOrDefault(p => p.Id == blackPlayerId);

            if (whitePlayer == null || blackPlayer == null)
                throw new Exception("Joueur introuvable");

            // Générer un ID unique pour la partie
            int newGameId = 1;
            foreach (var comp in Competitions)
            {
                if (comp.Games.Count > 0)
                {
                    int maxId = comp.Games.Max(g => g.Id);
                    if (maxId >= newGameId)
                        newGameId = maxId + 1;
                }
            }

            CurrentGame = new Game
            {
                Id = newGameId,
                CompetitionId = competitionId,
                WhitePlayerId = whitePlayerId,
                BlackPlayerId = blackPlayerId,
                GameDate = DateTime.Now,
                WhiteEloBeforeGame = whitePlayer.EloRating,
                BlackEloBeforeGame = blackPlayer.EloRating
            };
        }

        // Ajoute un coup à la partie en cours
        public void AddMove(string notation)
        {
            if (CurrentGame == null)
                throw new Exception("Aucune partie en cours");

            if (string.IsNullOrWhiteSpace(notation))
                throw new Exception("La notation ne peut pas être vide");

            var move = new Move(notation);
            CurrentGame.AddMove(move);

            OnPropertyChanged(nameof(CurrentGame));


            NewMoveNotation = string.Empty;
        }

        // Termine la partie et met à jour les ELO
        public void FinishGame(GameResult result)
        {
            if (CurrentGame == null)
                throw new Exception("Aucune partie en cours");

            CurrentGame.Result = result;

            // Trouver les joueurs
            var whitePlayer = Players.FirstOrDefault(p => p.Id == CurrentGame.WhitePlayerId);
            var blackPlayer = Players.FirstOrDefault(p => p.Id == CurrentGame.BlackPlayerId);

            if (whitePlayer != null && blackPlayer != null)
            {
                // Calculer les nouveaux ELO
                double whiteScore = CurrentGame.GetWhiteScore();
                double blackScore = CurrentGame.GetBlackScore();

                int newWhiteElo = _eloCalculator.CalculateNewElo(
                    CurrentGame.WhiteEloBeforeGame,
                    CurrentGame.BlackEloBeforeGame,
                    whiteScore
                );

                int newBlackElo = _eloCalculator.CalculateNewElo(
                    CurrentGame.BlackEloBeforeGame,
                    CurrentGame.WhiteEloBeforeGame,
                    blackScore
                );

                // Mettre à jour les joueurs
                whitePlayer.EloRating = newWhiteElo;
                blackPlayer.EloRating = newBlackElo;

                // Sauvegarder les joueurs
                _dataService.SavePlayers(Players.ToList());
            }

            // Ajouter la partie à la compétition
            var competition = Competitions.FirstOrDefault(c => c.Id == CurrentGame.CompetitionId);
            if (competition != null)
            {
                competition.Games.Add(CurrentGame);
                _dataService.SaveCompetitions(Competitions.ToList());
            }

            // Réinitialiser
            CurrentGame = null;
        }

        // Annule la partie en cours
        public void CancelGame()
        {
            CurrentGame = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
