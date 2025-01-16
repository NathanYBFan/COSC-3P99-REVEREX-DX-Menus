using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Nathan Fan
/// Description: Main menu controller which ensures a button is always selected for controller support
/// </summary>
public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionTextBox;
    [SerializeField] private GameObject firstSelectedButton;
    private GameObject lastSelected;

    /// <summary>
    /// Initializes the first button to be selected
    /// </summary>
    private void OnEnable()
    {
        descriptionTextBox.text = string.Empty;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    /// <summary>
    /// Ensures a button is always selected
    /// </summary>
    private void Update()
    {
        // Only run if this is the main scene
        if (SceneManager.GetSceneByName("CreditsMenu").isLoaded) return;
        if (SceneManager.GetSceneByName("OptionsMenu").isLoaded) return;

        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelected);
        else if (!SceneManager.GetSceneByName("CreditsMenu").isLoaded)
            lastSelected = EventSystem.current.currentSelectedGameObject;
     
    }

    #region Button Methods
    /// <summary>
    /// Updates the visual text box given a string input
    /// </summary>
    /// <param name="text">Text to display</param>
    public void UpdateTextDescription(string text)
    {
        descriptionTextBox.text = text;
    }

    /// <summary>
    /// Button behaviour if options button is pressed
    /// </summary>
    public void OpenOptionsMenu()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        LevelLoader.LoadAdditively("OptionsMenu");
    }

    /// <summary>
    /// Button behaviour if credits button is pressed
    /// </summary>
    public void OpenCreditsMenu()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        LevelLoader.Load("CreditsMenu");
    }

    /// <summary>
    /// Button behaviour if local game button is pressed
    /// </summary>
    public void LoadLocalGame()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        LoadLevel("WaitingLocal");
    }

    /// <summary>
    /// Button behaviour if networked game button is pressed
    /// </summary>
    public void LoadOnlineGame()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        LoadLevel("WaitingNet");
    }

    /// <summary>
    /// Button behaviour if replay button is pressed
    /// Force the game into play with a certain demo selected.
    /// </summary>
    public void PlayReplay()
    {
        if (LevelLoader.IsLoading) { return; }
        ControllerManager.AssignNavigator(new GameObject().AddComponent<DummyController>());
        ControllerManager.AssignVitalist(new GameObject().AddComponent<DummyController>());
        EventManager.Instance.InvokeStart();
        GameManager.instance.Navigator.TogglePlayback();
        LoadLevel("Level1");
    }

    /// <summary>
    /// Button behaviour if quit button is pressed
    /// </summary>
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
    #endregion

    /// <summary>
    /// Base Level loading script
    /// </summary>
    /// <param name="level">name of the level to load</param>
    private void LoadLevel(string level)
    {
        LevelLoader.UnloadAll();
        LevelLoader.LoadAdditively(level);
    }
}
