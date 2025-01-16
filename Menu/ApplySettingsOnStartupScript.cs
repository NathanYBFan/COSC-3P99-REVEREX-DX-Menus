using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

/// <summary>
/// Author: Nathan Fan
/// Description: Settings to apply to the game as it starts up
/// </summary>
public sealed class ApplySettingsOnStartupScript : MonoBehaviour
{
    [Space(5), Header("Audio Objects")]
    [SerializeField] private AudioMixer audioMixer;

    [Space(5), Header("Audio Levels")]
    public static float maxAudioVolume = -10f;

    [Space(5), Header("Video Settings")]
    [SerializeField] private ScriptableRendererData renderData;
    [SerializeField] private ScriptableRendererFeature gaussianSplatRenderFeature;
    [SerializeField] private ScriptableRendererFeature CRTRenderFeature;

    /// <summary>
    /// Initial setup of all variables
    /// </summary>
    private void Start()
    {
        // Apply audio Settings
        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("Master", maxAudioVolume));
        audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("Music", maxAudioVolume));
        audioMixer.SetFloat("Navigator", PlayerPrefs.GetFloat("Navigator", maxAudioVolume));
        audioMixer.SetFloat("Vitalist", PlayerPrefs.GetFloat("Vitalist", maxAudioVolume));
        audioMixer.SetFloat("Taffy", PlayerPrefs.GetFloat("Taffy", maxAudioVolume));
        audioMixer.SetFloat("Menu", PlayerPrefs.GetFloat("Menu", maxAudioVolume));

        // Apply Game Settings
        UniversalAdditionalCameraData uac = GameManager.Instance.Navigator.mainCamera.GetComponent<UniversalAdditionalCameraData>();
        uac.renderPostProcessing = PlayerPrefs.GetInt("PostToggle", 1) == 1;
        foreach (var rendererFeature in renderData.rendererFeatures)
        {
            if (rendererFeature == gaussianSplatRenderFeature)
                rendererFeature.SetActive(PlayerPrefs.GetInt("SplatsToggle", 1) == 1);
            else if (rendererFeature == CRTRenderFeature)
                rendererFeature.SetActive(PlayerPrefs.GetInt("PostToggle", 1) == 1);
        }
        
        // Apply Window state
        int valueA = PlayerPrefs.GetInt("WindowState", 0); // Save the value to the player prefs
        Screen.fullScreenMode = valueA switch
        {
            0 => // Borderless
                FullScreenMode.ExclusiveFullScreen,
            1 => // Windowed
                FullScreenMode.FullScreenWindow,
            2 => // Fullscreen
                FullScreenMode.Windowed,
            _ => Screen.fullScreenMode
        };

        // Apply Resolution
        // TODO : Match this with resolution options in options menu
        // We need to check if the window state is fullscreen, as we need to pass that to the Screen.SetResolution method
        bool fullscreen = PlayerPrefs.GetInt("windowState", 1) == 0 || PlayerPrefs.GetInt("windowState", 1) == 1;
        // Overwrite value variable

        Resolution[] res = Screen.resolutions;
        valueA = PlayerPrefs.GetInt("Resolution", res.Length);

        Resolution[] resolution = new Resolution[Screen.resolutions.Length];
        int count = 0;

        for (int i = res.Length - 1; i > 0; i--)
        {
            resolution[count] = res[i];
            count++;
        }
        Screen.SetResolution(resolution[valueA].width, resolution[valueA].height, fullscreen);

        // Remove this component, no need to re run this
        Destroy(this);
    }
}
