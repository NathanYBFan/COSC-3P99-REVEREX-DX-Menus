using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Nathan Fan
/// Description: Menu Controller for the credits menu: Sets up default selected buttons and button behaviours
/// </summary>
public class CreditsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    
    private GameObject lastSelectedButton;
    private GameObject lastSelected;

    /// <summary>
    /// Intialization of first selected buttons for controller support
    /// </summary>
    private void OnEnable()
    {
        lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        SteamAchievements.UnlockAchievement(AchievementIDs.NEW_ACHIEVEMENT_1_9.ToString());
    }

    /// <summary>
    /// Reset to previous menus button for controller support
    /// </summary>
    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }

    /// <summary>
    /// Game loop runs every frame to ensure a button is always selected
    /// </summary>
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelected);
        else
            lastSelected = EventSystem.current.currentSelectedGameObject;
    }

    /// <summary>
    /// Button Behaviour if the continue button is pressed: close the credits menu
    /// </summary>
    public void ContinueButtonPressed()
    {
        LevelLoader.Unload("CreditsMenu");
        LevelLoader.Load("MainMenu");
        EventSystem.current.SetSelectedGameObject(null);
    }
}
