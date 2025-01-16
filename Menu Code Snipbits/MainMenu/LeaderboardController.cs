using UnityEngine;
using System.Collections;
using Steamworks;

/// <summary>
/// Author: Nathan Fan
/// Description: Leaderboard element setup
/// </summary>
public class LeaderboardController : MonoBehaviour
{
    // List of all spawned entryHolders
    [SerializeField] private LeaderBoardEntry[] entryHolders;

    private bool isDataReady = false;

    /// <summary>
    /// Initialize all data and their visualizers
    /// </summary>
    private void Start()
    {
        InitializeEntryHolders();
        StartCoroutine(PollForLeaderboardData());
    }

    /// <summary>
    /// Setup default values for each visualizer
    /// </summary>
    private void InitializeEntryHolders()
    {
        foreach (var holder in entryHolders)
        {
            holder.SetValues(0, "Loading...", "Loading...");
        }
    }

    /// <summary>
    /// Set values in case of error/empty holders
    /// </summary>
    private void PopulateBlankEntryHolders()
    {
        for (int i = 0; i < entryHolders.Length; i++)
        {
            entryHolders[i].SetValues((i + 1), "Error", "Can't Find Steam");
        }
    }

    /// <summary>
    /// Waits for networked leaderboard data then populates the values into their visualizers
    /// </summary>
    /// <returns></returns>
    private IEnumerator PollForLeaderboardData()
    {
        float elapsedTime = 0f;
        float timeout = 5f;

        while (!isDataReady && elapsedTime < timeout)
        {
            if (Leaderboard.Instance.Entries.Length > 0)
            {
                UpdateUI();
                isDataReady = true;
            }

            elapsedTime += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        // Error case
        if (!isDataReady)
        {
            PopulateBlankEntryHolders();
            Debug.LogError("Timeout: Leaderboard data not found.");
        }
    }

    /// <summary>
    /// Update all visualizers upon data retrival
    /// </summary>
    private void UpdateUI()
    {
        for (int i = 0; i < entryHolders.Length; i++)
        {
            LeaderboardEntry entry = Leaderboard.Instance.Entries[i];
            if (!string.IsNullOrEmpty(entry.name))
            {
                if (entry.name == SteamFriends.GetPersonaName())
                {
                    SteamAchievements.UnlockAchievement(AchievementIDs.NEW_ACHIEVEMENT_1_2.ToString());
                    StatsTracker.TopFiveLeaderboard = true;
                }
                entryHolders[i].SetValues((i + 1), entry.time, entry.name);
            }
            else
            {
                entryHolders[i].SetValues((i + 1), "N/A", "N/A");
            }
        }
    }
}
