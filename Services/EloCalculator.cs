using System;

namespace Projet_Chess_db.Services
{

    public interface IEloCalculator
    {
        int CalculateNewElo(int currentElo, int opponentElo, double score);
        double CalculateExpectedScore(int playerElo, int opponentElo);
    }

    public class ChessEloCalculator : IEloCalculator
    {
        // Calcule le score attendu 
        public double CalculateExpectedScore(int playerElo, int opponentElo)
        {

            double exponent = (opponentElo - playerElo) / 400.0;
            double expectedScore = 1.0 / (1.0 + Math.Pow(10, exponent));
            return expectedScore;
        }

        // Calcule le nouveau ELO
        public int CalculateNewElo(int currentElo, int opponentElo, double score)
        {

            int kFactor = GetKFactor(currentElo);

            double expectedScore = CalculateExpectedScore(currentElo, opponentElo);
            double eloChange = kFactor * (score - expectedScore);

            int newElo = currentElo + (int)Math.Round(eloChange);

            if (newElo < 0)
                newElo = 0;

            return newElo;
        }

        // Méthode pour déterminer le K-factor selon l'ELO
        private int GetKFactor(int elo)
        {
            if (elo < 2000)
            {
                return 40;
            }
            else if (elo < 2400)
            {
                return 32;
            }
            else
            {
                return 16;
            }
        }
    }
}
