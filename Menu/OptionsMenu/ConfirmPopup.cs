using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Nathan Fan
/// Description: After detecting a change, a visual popup to confirm if you want to quit without saving or go back
/// </summary>
public class ConfirmPopup : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;

    private GameObject lastSelectedButton;
    private GameObject lastMenuButton;

    /// <summary>
    /// Initialize button selection for controller support
    /// </summary>
    private void OnEnable()
    {
        lastMenuButton = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    /// <summary>
    /// Reset button to last selected menu's button
    /// </summary>
    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(lastMenuButton);
    }

    /// <summary>
    /// Main game loop run every frame to ensure a button is always selected for controller support
    /// </summary>
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        else
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
    }

    /// <summary>
    /// Button behaviour if player pressed cancel: dont close without saving
    /// </summary>
    public void CancelButtonPressed()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Back");
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Button behaviour if player pressed Exit without saving: Close the menu and discard changes 
    /// </summary>
    public void ExitButtonPressed()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Back");
        GameManager.Instance.PauseLock = false;
        LevelLoader.Unload("OptionsMenu");
        LevelLoader.Unload("InGameOptionsMenu");
    }
}
