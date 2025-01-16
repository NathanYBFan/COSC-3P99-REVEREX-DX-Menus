using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Nathan Fan
/// Description: Local Game Character Select Menu Controller
/// </summary>
public class LocalPlayController : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private string levelToLoad = "Level1";
    [SerializeField] private PlayerSelector[] playerSelectors;
    [SerializeField] private Toggle skipTutorial;
    #endregion

    #region Private Variables
    private bool SkipTutorial { get { return skipTutorial.isOn; }}
    private bool called = false;
    #endregion

    #region Button Methods
    /// <summary>
    /// On Play button pressed, run correct methods and transition scenes
    /// </summary>
    public void PlayButtonPressed()
    {
        // Basic checks on when the play button is pressed
        if (LevelLoader.IsLoading) { return; }
        if (playerSelectors[0].currentMenuState != PlayerSelector.MenuState.Ready || playerSelectors[1].currentMenuState != PlayerSelector.MenuState.Ready) return;
        if (ControllerManager.VitalistController == null || ControllerManager.NavigatorController == null) return;
        if (ControllerManager.NavigatorController == ControllerManager.VitalistController) return;

        // This button should only call the functions once
        if (called) return;
        called = true;

        // Assign Roles
        if (ControllerManager.PlayerA != null && ControllerManager.NavigatorController == null)
            ControllerManager.AssignNavigator(ControllerManager.PlayerA);
        else if (ControllerManager.PlayerB != null && ControllerManager.VitalistController == null)
            ControllerManager.AssignVitalist(ControllerManager.PlayerB);

        // Load into correct positions
        if (SkipTutorial) 
        {
            GameManager.Instance.Navigator.transform.position = GameManager.Instance.TutorialSkipPoint.position;
            GameManager.Instance.Navigator.transform.rotation = GameManager.Instance.TutorialSkipPoint.rotation;
            GameManager.Instance.VitalistBody.SetActive(true);
            GameManager.Instance.Vitalist.unlockButton.Invoke(1);
            GameManager.Instance.Vitalist.unlockButton.Invoke(2);
            GameManager.Instance.Vitalist.TutorialMode = false;
        }
        else
        {
            GameManager.Instance.Vitalist.TutorialMode = true;
        }

        // Load into correct level
        LevelLoader.Load(levelToLoad);
    }

    /// <summary>
    /// On Back button pressed, run correct methods and transition scenes
    /// </summary>
    public void BackButtonPressed()
    {
        if (called) return;
        called = true;

        SteamLobby.Instance?.Cleanup();
        GameManager.Instance.RestartGame();
    }

    /// <summary>
    /// Debug force into play scene mode even if controllers are not setup
    /// </summary>
    public void ForcePlayButton()
    {
        if (LevelLoader.IsLoading) { return; }
        ControllerManager.AssignNavigator(ControllerManager.PlayerA);
        ControllerManager.AssignVitalist(ControllerManager.PlayerB);
        LevelLoader.Load(levelToLoad);
    }
    #endregion
}
