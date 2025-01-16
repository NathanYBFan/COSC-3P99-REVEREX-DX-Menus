using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Author: Nathan Fan
/// Description: On Button Select & Deselect Behaviour
/// </summary>
[RequireComponent(typeof(Button))]
public class MainMenuButtonSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private MainMenuController mainMenuController;
    [SerializeField] private Image selectedArrowImage;
    [SerializeField] private string buttonDescription;

    /// <summary>
    /// On selecting the button, activate arrow image, update text and play sound fx
    /// </summary>
    /// <param name="eventData">On button select data</param>
    public void OnSelect(BaseEventData eventData)
    {
        selectedArrowImage.gameObject.SetActive(true);
        mainMenuController.UpdateTextDescription(buttonDescription);
        SoundManager.Instance.PlaySFXOnline("Menu_Select");
    }

    /// <summary>
    /// On deselecting the button, deactivates the arrow.
    /// No need to remove text as they will have selected another button which will naturally update the text
    /// </summary>
    /// <param name="eventData">On button deselect data</param>
    public void OnDeselect(BaseEventData eventData)
    {
        selectedArrowImage.gameObject.SetActive(false);
    }
}
