using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Author: Nathan Fan
/// Description: Saveable object of settings
/// </summary>
[Serializable]
public struct SteamSettings
{
    public float MouseSense;
    public float ControllerSense;
    public bool AutoSprint;

    /// <summary>
    /// Constructor to save the specified values
    /// </summary>
    /// <param name="mouseSens">Mouse sensitivity to save</param>
    /// <param name="controllerSens">Controller joystick sensitivity to save</param>
    /// <param name="autoSprint">Auto sprint toggle value</param>
    public SteamSettings(float mouseSens, float controllerSens, bool autoSprint)
    {
        MouseSense = mouseSens;
        ControllerSense = controllerSens;
        AutoSprint = autoSprint;
    }
}

/// <summary>
/// Author: Nathan Fan
/// Description: Controller for the options menu 
/// </summary>
public class OptionsMenuController : Reloadable
{
    #region Serialized Fields
    [Space(5), Header("General Items")]
    [SerializeField] private GameObject[] availableTabs;
    [SerializeField] private GameObject confirmMenuPopup;

    [Space(5), Header("Buttons")]
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject[] tabButtons;
    [SerializeField] private GameObject[] bottomButtons;

    [Space(5), Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider[] audioSliders;
    [SerializeField] private float maxAudioVolume = 0f;

    [Space(5), Header("Game Settings")]
    [SerializeField] private Slider[] sensitivitySliders;
    [SerializeField] private TMP_Dropdown sprintBehaviour;
    [SerializeField] private Toggle joystickSens;

    [Space(5), Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowStateDropdown;
    [SerializeField] private Toggle splatsToggle;
    [SerializeField] private Toggle postToggle;
    [SerializeField] private ScriptableRendererData renderData;
    [SerializeField] private ScriptableRendererFeature gaussianSplatRenderFeature;
    [SerializeField] private ScriptableRendererFeature CRTRenderFeature;

    [Space(5), Header("Visuals")]
    [SerializeField] private Image settingsBox;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectedColor;
    #endregion

    #region Private Variables
    private bool changeDetected;
    private GameObject lastSelectedButton;
    private GameObject lastmenuSelectedButton;
    private MenuState currentState;
    private CursorLockMode previousLockMode;
    private Resolution[] res;
    private Resolution[] resolution;
    #endregion

    #region Enums
    private enum MenuState
    {
        BUTTONS,
        ADJUSTABLES
    }
    #endregion

    /// <summary>
    /// On open initializations
    /// </summary>
    private new void OnEnable()
    {
        base.OnEnable();

        res = Screen.resolutions;
        resolution = new Resolution[Screen.resolutions.Length];
        // Enable Cursor Lock
        previousLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.Confined;

        // Menu State setup
        currentState = MenuState.BUTTONS;
        confirmMenuPopup.SetActive(false);

        // Steam Save data 
        SteamSettings savedData = OptionsDataSaver.GetOptionsData();

        // Display saved audio values
        float masterVolume;
        float musicVolume;
        float menuVolume;
        float vitalistVolume;
        float navigatorVolume;
        float taffyVolume;

        // Get values from audio mixer
        audioMixer.GetFloat("Master", out masterVolume);
        audioMixer.GetFloat("Music", out musicVolume);
        audioMixer.GetFloat("Menu", out menuVolume);
        audioMixer.GetFloat("Vitalist", out vitalistVolume);
        audioMixer.GetFloat("Navigator", out navigatorVolume);
        audioMixer.GetFloat("Taffy", out taffyVolume);

        // Display audio mixer values properly
        audioSliders[0].value = masterVolume;
        audioSliders[1].value = musicVolume;
        audioSliders[2].value = menuVolume;
        audioSliders[3].value = vitalistVolume;
        audioSliders[4].value = navigatorVolume;
        audioSliders[5].value = taffyVolume;

        // Display saved game settings
        sensitivitySliders[0].value = PlayerPrefs.GetFloat("ControllerSens", 1);
        sensitivitySliders[1].value = PlayerPrefs.GetFloat("MouseSens", 1);
        sprintBehaviour.value = PlayerPrefs.GetInt("SprintBehaviour", 0);
        splatsToggle.isOn = PlayerPrefs.GetInt("SplatsToggle", 1) == 1;

        UniversalAdditionalCameraData uac = GameManager.Instance.Navigator.mainCamera.GetComponent<UniversalAdditionalCameraData>();
        postToggle.isOn = uac.renderPostProcessing;

        // Display saved visual values
        postToggle.isOn = PlayerPrefs.GetInt("PostToggle", 1) == 1;
        windowStateDropdown.value = PlayerPrefs.GetInt("WindowState", 2);

        ShowATab(0);
    }

    /// <summary>
    /// Reset all necessary elements
    /// </summary>
    private new void OnDisable()
    {
        base.OnDisable();

        Cursor.lockState = previousLockMode;
        EventSystem.current.SetSelectedGameObject(lastmenuSelectedButton);
    }

    /// <summary>
    /// On a system reload, reset button selection
    /// </summary>
    public override void OnReload()
    {
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }

    /// <summary>
    /// Less immediate initializations
    /// </summary>
    private void Start()
    {
        List<string> resolutionsText = new List<string>();
        changeDetected = false;
        int count = 0;
        
        for (int i = res.Length - 1; i > 0; i--)
        {
            resolutionsText.Add(res[i].width + "x" + res[i].height + "p" + " " + res[i].refreshRateRatio + "hz");
            //resolution = new Resolution[res.Length];
            resolution[count] = res[i];


            count++;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionsText);

        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", res.Length);
        // Last menu button configurations
        lastmenuSelectedButton = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    /// <summary>
    /// Main game loop that runs every frame to ensure a button is always selected for controller support
    /// </summary>
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        else
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;

        if (GetCancelPressed())
            ProcessCancel();   
    }

    /// <summary>
    /// If the cancel button is pressed, context sensitive behaviours
    /// </summary>
    private void ProcessCancel()
    {
        if (currentState == MenuState.ADJUSTABLES)
        {
            settingsBox.color = baseColor;
            for (int i = 0; i < availableTabs.Length; i++)
            {
                if (availableTabs[i].activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(tabButtons[i]);
                    currentState = MenuState.BUTTONS;
                    return;
                }
            }
        }
    }

    #region Button Pressed Methods
    /// <summary>
    /// On sound tab button pressed behaviour
    /// </summary>
    public void SoundTabPressed()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        settingsBox.color = selectedColor;
        currentState = MenuState.ADJUSTABLES;
        EventSystem.current.SetSelectedGameObject(audioSliders[0].gameObject);
    }

    /// <summary>
    /// On game tab pressed behaviour
    /// </summary>
    public void GameTabPressed()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        settingsBox.color = selectedColor;
        currentState = MenuState.ADJUSTABLES;
        EventSystem.current.SetSelectedGameObject(sensitivitySliders[0].gameObject);
    }

    /// <summary>
    /// On video tab pressed behaviour
    /// </summary>
    public void VideoTabPressed()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        settingsBox.color = selectedColor;
        currentState = MenuState.ADJUSTABLES;
        EventSystem.current.SetSelectedGameObject(resolutionDropdown.gameObject);
    }

    /// <summary>
    /// On apply settings button pressed behaviour
    /// </summary>
    public void ApplySettingsButton()
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Confirm");
        changeDetected = false;
        ApplyAudioSettings();
        ApplyGameSettings();
        ApplyVideoSettings();
    }

    /// <summary>
    /// On exit button pressed behaviour
    /// </summary>
    public void ExitButtonPressed()
    {
        // If change detected ask if they want to save
        if(changeDetected)
        {
            // Open confirm menu
            SoundManager.Instance.PlaySFXOnline("Menu_Select");
            confirmMenuPopup.SetActive(true);
            return;
        }

        GameManager.Instance.PauseLock = false;
        LevelLoader.Unload("InGameOptionsMenu");
        LevelLoader.Unload("OptionsMenu");
    }

    /// <summary>
    /// If a value has been changed, recognize that
    /// </summary>
    public void SomethingAdjusted()
    {
        changeDetected = true;
    }
    #endregion

    #region Reuseable Methods
    /// <summary>
    /// Close all settings menu tabs
    /// </summary>
    public void CloseAllTabs()
    {
        foreach(var item in availableTabs)
            item.SetActive(false);
    }

    /// <summary>
    /// Show a single settings menu tab
    /// </summary>
    /// <param name="tabToOpen">tab to open from the list</param>
    public void ShowATab(int tabToOpen)
    {
        if (tabToOpen < 0 || tabToOpen > availableTabs.Length) return;
        CloseAllTabs();
        availableTabs[tabToOpen].SetActive(true);
    }

    /// <summary>
    /// Apply changed audio settings
    /// </summary>
    private void ApplyAudioSettings()
    {
        // Save value
        PlayerPrefs.SetFloat("Master", audioSliders[0].value);
        PlayerPrefs.SetFloat("Music", audioSliders[1].value);
        PlayerPrefs.SetFloat("Menu", audioSliders[2].value);
        PlayerPrefs.SetFloat("Vitalist", audioSliders[3].value);
        PlayerPrefs.SetFloat("Navigator", audioSliders[4].value);
        PlayerPrefs.SetFloat("Taffy", audioSliders[5].value);

        // Apply values to mixer
        audioMixer.SetFloat("Master", audioSliders[0].value);
        audioMixer.SetFloat("Music", audioSliders[1].value);
        audioMixer.SetFloat("Menu", audioSliders[2].value);
        audioMixer.SetFloat("Vitalist", audioSliders[3].value);
        audioMixer.SetFloat("Navigator", audioSliders[4].value);
        audioMixer.SetFloat("Taffy", audioSliders[5].value);
    }

    /// <summary>
    /// Apply changed Game settings 
    /// </summary>
    private void ApplyGameSettings()
    {
        PlayerPrefs.SetFloat("ControllerSens", sensitivitySliders[0].value);
        PlayerPrefs.SetFloat("MouseSens", sensitivitySliders[1].value);
        PlayerPrefs.SetInt("SprintBehaviour", sprintBehaviour.value);
        PlayerPrefs.SetInt("SplatsToggle", splatsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("PostToggle", postToggle.isOn ? 1 : 0);

        foreach (var rendererFeature in renderData.rendererFeatures)
        {
            if (rendererFeature == gaussianSplatRenderFeature)
                rendererFeature.SetActive(splatsToggle.isOn);
            else if (rendererFeature == CRTRenderFeature)
                rendererFeature.SetActive(postToggle.isOn);
        }

        UniversalAdditionalCameraData uac = GameManager.Instance.Navigator.mainCamera.GetComponent<UniversalAdditionalCameraData>();
        uac.renderPostProcessing = postToggle.isOn;
    }

    /// <summary>
    /// Apply changed video settings
    /// </summary>
    private void ApplyVideoSettings()
    {
        int value = windowStateDropdown.value;

        PlayerPrefs.SetInt("WindowState", value); // Save the value to the player prefs

        Screen.fullScreenMode = value switch
        {
            0 => // Borderless
                FullScreenMode.ExclusiveFullScreen,
            1 => // Windowed
                FullScreenMode.FullScreenWindow,
            2 => // Fullscreen
                FullScreenMode.Windowed,
            _ => Screen.fullScreenMode
        };

        value = resolutionDropdown.value;
        // We need to check if the window state is fullscreen, as we need to pass that to the Screen.SetResolution method
        bool fullscreen = PlayerPrefs.GetInt("windowState", 1) == 0 || PlayerPrefs.GetInt("windowState", 1) == 1;

        // Overwrite value variable
        PlayerPrefs.SetInt("Resolution", value);

        Screen.SetResolution(resolution[value].width, resolution[value].height, fullscreen);
    }

    /// <summary>
    /// Gets if UI cancel button has been pressed
    /// </summary>
    /// <returns>If cancel is pressed</returns>
    private bool GetCancelPressed()
    {
        if (ControllerManager.PlayerA != null && ControllerManager.PlayerA.UICancel)
            return true;
        
        if (ControllerManager.PlayerB != null && ControllerManager.PlayerB.UICancel)
            return true;

        return false;
    }
    #endregion
}
