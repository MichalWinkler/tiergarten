using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages synchronization of player gameplay stats and remote leaderboard with Supabase (PostgreSQL).
/// </summary>
public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance { get; private set; }

    [Header("Supabase Configuration")]
    [Tooltip("Your Supabase Project URL (e.g. https://your-project.supabase.co)")]
    [SerializeField] private string supabaseUrl = "https://YOUR_PROJECT_ID.supabase.co";

    [Tooltip("Your Supabase anon public API Key")]
    [SerializeField] private string supabaseAnonKey = "YOUR_ANON_KEY";

    [Header("Player Settings")]
    [Tooltip("Unique username for this player. Used as the primary key in PostgreSQL.")]
    [SerializeField] private string playerName = "Player1";

    [Header("Leaderboard & Save Options")]
    [Tooltip("Automatically save progress when closing the game")]
    [SerializeField] private bool autoSaveOnQuit = true;

    [Tooltip("Show an interactive in-game test overlay to test Save & Leaderboard")]
    [SerializeField] private bool showDebugUI = true;

    // Runtime state
    private float sessionStartTime;
    private int existingPlaytimeSeconds = 0;
    private bool isSaving = false;
    private bool isLoadingLeaderboard = false;
    private string statusMessage = "Ready";
    private List<PlayerRecord> leaderboard = new List<PlayerRecord>();

    [Serializable]
    public class PlayerRecord
    {
        public string player_name;
        public int pet_count;
        public int playtime_seconds;
        public string last_online_at;
    }

    [Serializable]
    private class PlayerRecordArrayWrapper
    {
        public PlayerRecord[] Items;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        sessionStartTime = Time.time;

        // Give default random name if left default
        if (string.IsNullOrEmpty(playerName) || playerName == "Player1")
        {
            playerName = "Player_" + UnityEngine.Random.Range(100, 999);
        }

        // Fetch existing stats (to accumulate playtime) and initial leaderboard
        StartCoroutine(FetchExistingPlayerRoutine());
        StartCoroutine(FetchLeaderboardRoutine());
    }

    void Update()
    {
        // Toggle debug UI with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showDebugUI = !showDebugUI;
        }
    }

    /// <summary>
    /// Counts how many pet objects are currently in the scene.
    /// </summary>
    public int GetCurrentPetCount()
    {
        pet[] pets = UnityEngine.Object.FindObjectsByType<pet>(FindObjectsSortMode.None);
        return pets != null ? pets.Length : 0;
    }

    /// <summary>
    /// Gets total playtime (accumulated previous sessions + current session).
    /// </summary>
    public int GetTotalPlaytimeSeconds()
    {
        int currentSessionSeconds = Mathf.RoundToInt(Time.time - sessionStartTime);
        return existingPlaytimeSeconds + currentSessionSeconds;
    }

    /// <summary>
    /// Saves current player stats (name, pet count, total playtime, last online) to PostgreSQL.
    /// </summary>
    public void SaveData()
    {
        if (isSaving) return;
        StartCoroutine(SavePlayerDataRoutine(GetCurrentPetCount()));
    }

    /// <summary>
    /// Loads the top 10 players sorted by pet count.
    /// </summary>
    public void RefreshLeaderboard()
    {
        if (isLoadingLeaderboard) return;
        StartCoroutine(FetchLeaderboardRoutine());
    }

    private IEnumerator FetchExistingPlayerRoutine()
    {
        if (string.IsNullOrEmpty(supabaseUrl) || supabaseUrl.Contains("YOUR_PROJECT_ID"))
        {
            statusMessage = "Please configure Supabase URL and Key in the Inspector.";
            yield break;
        }

        string url = $"{supabaseUrl.TrimEnd('/')}/rest/v1/players?player_name=eq.{UnityWebRequest.EscapeURL(playerName)}&select=*";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            SetSupabaseHeaders(request);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerRecord[] records = ParseRecords(request.downloadHandler.text);
                if (records != null && records.Length > 0)
                {
                    existingPlaytimeSeconds = records[0].playtime_seconds;
                    statusMessage = $"Welcome back, {playerName}! Existing playtime: {FormatTime(existingPlaytimeSeconds)}";
                }
                else
                {
                    statusMessage = $"New player '{playerName}' registered.";
                }
            }
        }
    }

    private IEnumerator SavePlayerDataRoutine(int petCount)
    {
        if (string.IsNullOrEmpty(supabaseUrl) || supabaseUrl.Contains("YOUR_PROJECT_ID"))
        {
            statusMessage = "Cannot save: Supabase credentials missing.";
            yield break;
        }

        isSaving = true;
        statusMessage = "Saving data to PostgreSQL...";

        PlayerRecord record = new PlayerRecord
        {
            player_name = this.playerName,
            pet_count = petCount,
            playtime_seconds = GetTotalPlaytimeSeconds(),
            last_online_at = DateTime.UtcNow.ToString("o") // ISO 8601 UTC
        };

        string json = JsonUtility.ToJson(record);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string endpoint = $"{supabaseUrl.TrimEnd('/')}/rest/v1/players";

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            SetSupabaseHeaders(request);

            // Upsert header: if player_name exists, update row instead of erroring
            request.SetRequestHeader("Prefer", "resolution=merge-duplicates");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusMessage = $"Successfully saved at {DateTime.Now:HH:mm:ss}!";
                Debug.Log($"<color=green>[Supabase]</color> Saved: {playerName}, Pets: {petCount}, Playtime: {record.playtime_seconds}s");
                // Refresh leaderboard so player sees updated position
                StartCoroutine(FetchLeaderboardRoutine());
            }
            else
            {
                statusMessage = $"Save error: {request.error}";
                Debug.LogError($"[Supabase Save Error] {request.error}: {request.downloadHandler.text}");
            }
        }

        isSaving = false;
    }

    private IEnumerator FetchLeaderboardRoutine()
    {
        if (string.IsNullOrEmpty(supabaseUrl) || supabaseUrl.Contains("YOUR_PROJECT_ID"))
        {
            yield break;
        }

        isLoadingLeaderboard = true;
        // Query top 10 sorted by pet_count descending
        string url = $"{supabaseUrl.TrimEnd('/')}/rest/v1/players?select=player_name,pet_count,playtime_seconds,last_online_at&order=pet_count.desc&limit=10";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            SetSupabaseHeaders(request);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerRecord[] records = ParseRecords(request.downloadHandler.text);
                leaderboard.Clear();
                if (records != null)
                {
                    leaderboard.AddRange(records);
                }
                statusMessage = $"Leaderboard updated ({leaderboard.Count} players).";
            }
            else
            {
                Debug.LogWarning($"[Supabase Leaderboard] Could not load: {request.error}");
            }
        }

        isLoadingLeaderboard = false;
    }

    private void SetSupabaseHeaders(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", supabaseAnonKey);
        request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");
    }

    private PlayerRecord[] ParseRecords(string json)
    {
        try
        {
            string wrapped = "{\"Items\":" + json + "}";
            PlayerRecordArrayWrapper wrapper = JsonUtility.FromJson<PlayerRecordArrayWrapper>(wrapped);
            return wrapper != null ? wrapper.Items : new PlayerRecord[0];
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing JSON: {ex.Message}\nRaw JSON: {json}");
            return new PlayerRecord[0];
        }
    }

    private string FormatTime(int seconds)
    {
        int mins = seconds / 60;
        int secs = seconds % 60;
        return $"{mins}m {secs:D2}s";
    }

    void OnApplicationQuit()
    {
        if (autoSaveOnQuit)
        {
            SaveData();
        }
    }

    // Interactive in-game test overlay
    void OnGUI()
    {
        if (!showDebugUI) return;

        GUI.Box(new Rect(10, 10, 320, 390), "<b>Tiergarten Database Sync</b>");

        GUILayout.BeginArea(new Rect(20, 35, 300, 355));

        GUILayout.Label($"<b>Player:</b> {playerName}");
        GUILayout.Label($"<b>Pets in Scene:</b> {GetCurrentPetCount()}");
        GUILayout.Label($"<b>Total Playtime:</b> {FormatTime(GetTotalPlaytimeSeconds())}");
        GUILayout.Label($"<size=10>Status: {statusMessage}</size>");

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(isSaving ? "Saving..." : "Save Progress", GUILayout.Height(30)))
        {
            SaveData();
        }

        if (GUILayout.Button(isLoadingLeaderboard ? "Loading..." : "Refresh Board", GUILayout.Height(30)))
        {
            RefreshLeaderboard();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("<b>🏆 Top Pet Owners (Remote Leaderboard):</b>");

        if (leaderboard.Count == 0)
        {
            GUILayout.Label("<color=grey>No entries yet or not connected.</color>");
        }
        else
        {
            for (int i = 0; i < leaderboard.Count; i++)
            {
                var entry = leaderboard[i];
                string isMe = entry.player_name == playerName ? " <b>(You)</b>" : "";
                GUILayout.Label($"#{i + 1} {entry.player_name}{isMe} - {entry.pet_count} pets ({FormatTime(entry.playtime_seconds)})");
            }
        }

        GUILayout.Space(5);
        GUILayout.Label("<size=9>Press <b>Tab</b> to toggle this overlay</size>");

        GUILayout.EndArea();
    }
}
