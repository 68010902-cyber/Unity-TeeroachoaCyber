using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public const float MatchDurationSeconds = 60f;

    public sealed class RunRecord
    {
        public string Player { get; }
        public string Level { get; }
        public int Score { get; }

        public RunRecord(string player, string level, int score)
        {
            Player = player;
            Level = level;
            Score = score;
        }
    }

    private static readonly List<RunRecord> runHistory = new List<RunRecord>();

    public static string username = "Player";
    public static int score;
    public static int bestScore;
    public static int combo;
    public static float timeRemaining = MatchDurationSeconds;
    public static FruitSpeedLevel selectedLevel = FruitSpeedLevel.Normal;
    public static Sprite selectedBackgroundSprite;
    public static IReadOnlyList<RunRecord> RunHistory => runHistory;

    public static void ResetForNewPlayer()
    {
        username = "Player";
        score = 0;
        bestScore = 0;
        combo = 0;
        timeRemaining = MatchDurationSeconds;
        selectedLevel = FruitSpeedLevel.Normal;
        runHistory.Clear();
    }

    public static void UpdateBestScore()
    {
        if (score > bestScore)
            bestScore = score;
    }

    public static void RecordCompletedRun()
    {
        UpdateBestScore();
        runHistory.Add(new RunRecord(username, selectedLevel.ToString(), score));
    }
}