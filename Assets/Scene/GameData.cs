using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public const float MatchDurationSeconds = 60f;
    public const int StartingLives = 3;

    public sealed class RunRecord
    {
        public string RunId { get; }
        public string Player { get; }
        public string Level { get; }
        public int Score { get; }
        public float DurationSeconds { get; }

        public RunRecord(string runId, string player, string level, int score, float durationSeconds)
        {
            RunId = runId;
            Player = player;
            Level = level;
            Score = score;
            DurationSeconds = durationSeconds;
        }
    }

    private static readonly List<RunRecord> runHistory = new List<RunRecord>();
    private static RunRecord pendingCompletedRun;

    public static bool HasRetriedThisSession { get; private set; }
    public static string username = "Player";
    public static int score;
    public static int bestScore;
    public static int combo;
    public static int lives = StartingLives;
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
        lives = StartingLives;
        timeRemaining = MatchDurationSeconds;
        selectedLevel = FruitSpeedLevel.Normal;
        runHistory.Clear();
        pendingCompletedRun = null;
        HasRetriedThisSession = false;
    }

    public static void MarkRetryUsed()
    {
        HasRetriedThisSession = true;
    }

    public static void UpdateBestScore()
    {
        if (score > bestScore)
            bestScore = score;
    }

    public static RunRecord PrepareCompletedRun()
    {
        UpdateBestScore();
        float durationSeconds = Mathf.Clamp(MatchDurationSeconds - timeRemaining, 0f, MatchDurationSeconds);
        pendingCompletedRun = new RunRecord(
            System.Guid.NewGuid().ToString("N"),
            username,
            selectedLevel.ToString(),
            score,
            durationSeconds
        );
        return pendingCompletedRun;
    }

    public static RunRecord RecordPendingCompletedRun()
    {
        if (pendingCompletedRun == null)
            return null;

        RunRecord record = pendingCompletedRun;
        pendingCompletedRun = null;
        runHistory.Add(record);
        return record;
    }
}