using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Nathan Fan
/// Description: Options menu additional behaviour scripting
/// </summary>
public class SoundOptionSelector : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text textbox;
    [SerializeField] private GameObject arrowImage;
    [SerializeField] private GameObject handleBar;

    [Space(5), Header("Select/Deselect Colors")]
    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectedColor;

    /// <summary>
    /// On button select behaviour
    /// </summary>
    /// <param name="eventData">On button select event data</param>
    public void OnSelect(BaseEventData eventData)
    {
        textbox.fontStyle = FontStyles.Bold;
        textbox.color = selectedColor;

        arrowImage.SetActive(true);
        if (handleBar != null)
            handleBar.transform.localScale = Vector3.one * 1.5f;
    }

    /// <summary>
    /// On button deselect behaviour
    /// </summary>
    /// <param name="eventData">On button deselect event data</param>
    public void OnDeselect(BaseEventData eventData)
    {
        textbox.fontStyle = FontStyles.Normal;
        textbox.color = baseColor;

        arrowImage.SetActive(false);
        if (handleBar != null)
            handleBar.transform.localScale = Vector3.one;
    }
}
