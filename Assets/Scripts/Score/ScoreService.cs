using System;

namespace EndlessGame.Score
{
    public class ScoreService : IScoreService
    {
        private int currentScore = 0;

        public event Action<int> OnScoreChanged;
        public int CurrentScore => currentScore;

        public void SetScore(int amount)
        {
            currentScore = amount;
            OnScoreChanged?.Invoke(currentScore);
        }

        public void ResetScore()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
        }
    }


    public interface IScoreService
    {
        int CurrentScore { get; }
        event Action<int> OnScoreChanged;

        void SetScore(int amount);
        void ResetScore();

    }
}

