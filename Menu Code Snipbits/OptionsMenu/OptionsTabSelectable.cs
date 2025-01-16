using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Nathan Fan
/// Description: On tab select behaviour
/// </summary>
public class OptionsTabSelectable : MonoBehaviour, ISelectHandler
{
    [SerializeField] private int buttonToSelect;
    [SerializeField] private OptionsMenuController menuController;

    /// <summary>
    /// On button select behaviour: show proper tab
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        SoundManager.Instance.PlaySFXOnline("Menu_Select");
        menuController.ShowATab(buttonToSelect);
    }
}
