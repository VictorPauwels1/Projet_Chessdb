using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Projet_Chess_db.Models;

namespace Projet_Chess_db.Services
{
    // Interface pour la persistance
    public interface IDataService
    {
        List<Player> LoadPlayers();
        void SavePlayers(List<Player> players);
        List<Competition> LoadCompetitions();
        void SaveCompetitions(List<Competition> competitions);
    }

    public class JsonDataService : IDataService
    {
        private readonly string _dataFolder;


        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };


        public JsonDataService(string dataFolder = "Data")
        {
            _dataFolder = dataFolder;


            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
        }

        // Charge les joueurs depuis players.json
        public List<Player> LoadPlayers()
        {
            string filePath = Path.Combine(_dataFolder, "players.json");


            if (!File.Exists(filePath))
            {
                return new List<Player>();
            }

            try
            {
                // Lire le contenu du fichier
                string jsonContent = File.ReadAllText(filePath);


                List<Player> players = JsonSerializer.Deserialize<List<Player>>(jsonContent, _jsonOptions);

                return players ?? new List<Player>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur chargement: {ex.Message}");
                return new List<Player>();
            }
        }

        // Sauvegarde les joueurs dans players.json
        public void SavePlayers(List<Player> players)
        {
            string filePath = Path.Combine(_dataFolder, "players.json");

            try
            {

                string jsonContent = JsonSerializer.Serialize(players, _jsonOptions);

                // Écrire dans le fichier
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sauvegarde: {ex.Message}");
            }
        }

        // Charge les compétitions
        public List<Competition> LoadCompetitions()
        {
            string filePath = Path.Combine(_dataFolder, "competitions.json");

            if (!File.Exists(filePath))
            {
                return new List<Competition>();
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                List<Competition> competitions = JsonSerializer.Deserialize<List<Competition>>(jsonContent, _jsonOptions);
                return competitions ?? new List<Competition>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur chargement: {ex.Message}");
                return new List<Competition>();
            }
        }

        // Sauvegarde les compétitions
        public void SaveCompetitions(List<Competition> competitions)
        {
            string filePath = Path.Combine(_dataFolder, "competitions.json");

            try
            {
                string jsonContent = JsonSerializer.Serialize(competitions, _jsonOptions);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sauvegarde: {ex.Message}");
            }
        }
    }
}