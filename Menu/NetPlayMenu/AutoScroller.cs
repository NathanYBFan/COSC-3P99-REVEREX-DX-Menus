using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Author: Nathan Fan
/// Description: Autoscrolling the friends list within the net play role select menu
/// </summary>
public class AutoScroller : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameObject parentObject;

    /// <summary>
    /// On selection of the list element, scrolls the selector linearly when navigating up and down the friends list
    /// </summary>
    /// <param name="eventData">On button selected data</param>
    public void OnSelect(BaseEventData eventData)
    {
        // get dropdown object
        ScrollRect rect = gameObject.transform.parent.parent.parent.parent.GetComponent<ScrollRect>();
        
        float objectIndex = parentObject.transform.GetSiblingIndex(); // Get dropdown position
        float totalChildCount = parentObject.transform.parent.childCount;
        
        rect.verticalScrollbar.value = 1 - (objectIndex / (totalChildCount - 2));
    }
}
