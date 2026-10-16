using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class GoogleSheetsReporter : MonoBehaviour
{
    private const string WebAppUrl = "https://script.google.com/macros/s/AKfycbxIJngs_GH4SdXvBZf8iosFCJ2vBtKvaqinqKhYkZdcBW22P76Zo5lHa6j5BToBgkgf/exec";
    private const string PendingRecordsKey = "GoogleSheets.PendingRuns";
    private static GoogleSheetsReporter instance;

    [Serializable]
    private sealed class SheetRun
    {
        public string runId;
        public string player;
        public int score;
        public float durationSeconds;
        public string difficulty;
        public bool incrementPlayCount = true;

        public SheetRun(GameData.RunRecord record, bool shouldIncrementPlayCount)
        {
            runId = record.RunId;
            player = record.Player;
            score = record.Score;
            durationSeconds = record.DurationSeconds;
            difficulty = record.Level;
            incrementPlayCount = shouldIncrementPlayCount;
        }
    }

    [Serializable]
    private sealed class PendingRuns
    {
        public List<SheetRun> records = new List<SheetRun>();
    }

    [Serializable]
    private sealed class SheetResponse
    {
        public bool success;
        public string message;
    }

    private PendingRuns pendingRuns;
    private Coroutine uploadCoroutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateReporter()
    {
        if (instance != null)
            return;

        GameObject reporterObject = new GameObject(nameof(GoogleSheetsReporter));
        DontDestroyOnLoad(reporterObject);
        instance = reporterObject.AddComponent<GoogleSheetsReporter>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadPendingRuns();
    }

    private void Start()
    {
        ProcessPendingRuns();
    }

    public static void Submit(GameData.RunRecord record, bool incrementPlayCount)
    {
        if (record == null)
        {
            Debug.LogError("Cannot submit a null completed game record.");
            return;
        }

        if (instance == null)
            CreateReporter();

        instance.LoadPendingRuns();
        instance.pendingRuns.records.Add(new SheetRun(record, incrementPlayCount));
        instance.SavePendingRuns();
        instance.ProcessPendingRuns();
    }

    private void LoadPendingRuns()
    {
        if (pendingRuns != null)
            return;

        string savedRuns = PlayerPrefs.GetString(PendingRecordsKey, string.Empty);
        pendingRuns = string.IsNullOrEmpty(savedRuns)
            ? new PendingRuns()
            : JsonUtility.FromJson<PendingRuns>(savedRuns);

        if (pendingRuns == null || pendingRuns.records == null)
        {
            Debug.LogError("Could not read the saved Google Sheets upload queue.");
            pendingRuns = new PendingRuns();
        }
    }

    private void SavePendingRuns()
    {
        PlayerPrefs.SetString(PendingRecordsKey, JsonUtility.ToJson(pendingRuns));
        PlayerPrefs.Save();
    }

    private void ProcessPendingRuns()
    {
        if (uploadCoroutine != null || pendingRuns.records.Count == 0)
            return;

        if (string.IsNullOrWhiteSpace(WebAppUrl))
        {
            Debug.LogError("Google Sheets is not configured. Set WebAppUrl in GoogleSheetsReporter.cs.");
            return;
        }

        uploadCoroutine = StartCoroutine(UploadPendingRuns());
    }

    private IEnumerator UploadPendingRuns()
    {
        while (pendingRuns.records.Count > 0)
        {
            SheetRun run = pendingRuns.records[0];
            byte[] body = Encoding.UTF8.GetBytes(JsonUtility.ToJson(run));

            using (UnityWebRequest request = new UnityWebRequest(WebAppUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(
                        "Google Sheets upload failed. The record remains queued for the next attempt. " +
                        request.error
                    );
                    uploadCoroutine = null;
                    yield break;
                }

                SheetResponse response = JsonUtility.FromJson<SheetResponse>(request.downloadHandler.text);
                if (response == null || !response.success)
                {
                    string message = response == null ? "Invalid response from the Apps Script endpoint." : response.message;
                    Debug.LogError("Google Sheets rejected the record. The record remains queued. " + message);
                    uploadCoroutine = null;
                    yield break;
                }
            }

            pendingRuns.records.RemoveAt(0);
            SavePendingRuns();
        }

        uploadCoroutine = null;
    }
}
