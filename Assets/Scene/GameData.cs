using UnityEngine;

public static class GameData
{
    private const string BestScoreKey = "BestScore";
    public const float MatchDurationSeconds = 30f;

    public static string username = "Player";
    public static int score;
    public static int bestScore;
    public static float timeRemaining = MatchDurationSeconds;
    public static FruitSpeedLevel selectedLevel = FruitSpeedLevel.Normal;

    static GameData()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    public static void ResetForNewPlayer()
    {
        username = "Player";
        score = 0;
        timeRemaining = MatchDurationSeconds;
        selectedLevel = FruitSpeedLevel.Normal;
    }

    public static void UpdateBestScore()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }
    }
}
