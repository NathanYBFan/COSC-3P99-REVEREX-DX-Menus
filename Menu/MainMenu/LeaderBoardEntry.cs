using TMPro;
using UnityEngine;

/// <summary>
/// Author: Nathan Fan
/// Description: Base Object of Leaderboard entries to hold basic data
/// </summary>
public sealed class LeaderBoardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text userName;

    /// <summary>
    /// Updates values with appropriate formatting at the correct locations onto the visualizer when data is recieved
    /// </summary>
    /// <param name="rank">Current rank of entry</param>
    /// <param name="time">Time it took to beat the game</param>
    /// <param name="username">Steam publicly displayed username</param>
    public void SetValues(float rank, string time, string username)
    {
        rankText.text = rank.ToString() + ".";
        this.time.text = time;
        this.userName.text = username;
    }
}
