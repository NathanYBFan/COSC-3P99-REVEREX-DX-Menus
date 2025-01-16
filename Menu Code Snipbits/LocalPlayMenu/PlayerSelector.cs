using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Nathan Fan
/// Description: The Player Agnostic selector for the Local Game Character Select Menu
/// </summary>
public class PlayerSelector : MonoBehaviour
{
    #region Enums
    private enum Player
    {
        PlayerA,
        PlayerB
    };

    private enum ButtonLayers
    {
        RoleSelect,
        TutorialToggle,
        BackButton
    }

    public enum MenuState
    {
        Selecting,
        Ready
    }
    #endregion

    public MenuState currentMenuState = MenuState.Selecting;

    #region Serialized Variables
    [Space(10), Header("Objects")]
    [SerializeField] private GameObject selectorObject;
    [SerializeField] private GameObject[] selectables;

    [Space(10), Header("UI elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private TextMeshProUGUI selectorText;

    [Space(10), Header("Base variables")]
    [SerializeField] private Player selectedPlayer;
    [SerializeField] private float maxTimeBetweenInputs = 1f;
    [SerializeField] private LocalPlayController localPlayController;
    #endregion

    #region Private Variables
    private int selectedObjectIndex = 0;
    private ButtonLayers selectedLayer = ButtonLayers.RoleSelect;
    private float timer = 0;

    private Color selectColor = Color.green;
    private Color defaultColor = Color.white;
    #endregion

    /// <summary>
    /// Startup initializations
    /// </summary>
    private void Start()
    {
        selectorText.color = defaultColor;
        selectedObjectIndex = 0;
        selectedLayer = ButtonLayers.RoleSelect;
        currentMenuState = MenuState.Selecting;

        timer = maxTimeBetweenInputs;
        SelectObject(selectables[selectedObjectIndex]);
    }

    /// <summary>
    /// Update user input changes
    /// </summary>
    private void Update()
    {
        // Get player input
        Vector2 input = GetControllerDirectionInput();
        bool select = GetSelectPressed();
        bool cancel = GetCancelPressed();

        // If player input is empty (Not pressed) allow next input
        if (Mathf.RoundToInt(input.magnitude) == 0 && !select && !cancel)
        {
            timer = 0;
            return;
        }
        // If rewindTimer is not up incriment
        else if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        // Accept confirm/cancel input
        if (select) SelectPressed();
        if (cancel) DeselectPressed();

        // No changing location if locked in
        if (currentMenuState == MenuState.Ready) return;

        // Accept move input
        MoveLeftOrRight(input);
        MoveUpOrDown(input);
        
        timer = maxTimeBetweenInputs;
    }

    #region Get Input Methods
    /// <summary>
    /// Gets input direction in both x and y directions to navigate UI
    /// </summary>
    /// <returns>Calculated input direction</returns>
    private Vector2 GetControllerDirectionInput()
    {
        switch (selectedPlayer)
        {
            case Player.PlayerA:
                if (ControllerManager.PlayerA == null) return Vector2.zero;
                return ControllerManager.PlayerA.AnalogueAxis + ControllerManager.PlayerA.DirectionalAxis;
            case Player.PlayerB:
                if (ControllerManager.PlayerB == null) return Vector2.zero;
                return ControllerManager.PlayerB.AnalogueAxis + ControllerManager.PlayerB.DirectionalAxis;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// Gets if UI select button has been pressed
    /// </summary>
    /// <returns>If select is pressed</returns>
    private bool GetSelectPressed()
    {
        switch (selectedPlayer)
        {
            case Player.PlayerA:
                if (ControllerManager.PlayerA == null) return false;
                return ControllerManager.PlayerA.UISelect;
            case Player.PlayerB:
                if (ControllerManager.PlayerB == null) return false;
                return ControllerManager.PlayerB.UISelect;
        }
        return false;
    }

    /// <summary>
    /// Gets if UI cancel button has been pressed
    /// </summary>
    /// <returns>If cancel is pressed</returns>
    private bool GetCancelPressed()
    {
        switch (selectedPlayer)
        {
            case Player.PlayerA:
                if (ControllerManager.PlayerA == null) return false;
                return ControllerManager.PlayerA.UICancel;
            case Player.PlayerB:
                if (ControllerManager.PlayerB == null) return false;
                return ControllerManager.PlayerB.UICancel;
        }
        return false;
    }
    #endregion

    #region Input Post Press Methods
    /// <summary>
    /// Properly parent to selected object
    /// </summary>
    /// <param name="parent">Object to parent to</param>
    private void SelectObject(GameObject parent)
    {
        selectorObject.transform.SetParent(parent.transform);
        selectorObject.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Button Behaviour when selecting
    /// </summary>
    /// <param name="selection">Button to select</param>
    private void LockIn(int selection)
    {
        switch (selection)
        {
            case 0:             // Navigator
                if (ControllerManager.NavigatorController != null) return;
                if (selectedPlayer == Player.PlayerA) { 
                    ControllerManager.AssignNavigator(ControllerManager.PlayerA);
                    SoundManager.Instance.PlaySFXOnline("Menu_NavConnect");
                }
                else if (selectedPlayer == Player.PlayerB) {
                    ControllerManager.AssignNavigator(ControllerManager.PlayerB);
                    SoundManager.Instance.PlaySFXOnline("Menu_NavConnect");
                }
                break;
            case 1:             // Vitalist
                if (ControllerManager.VitalistController != null) return;
                if (selectedPlayer == Player.PlayerA)
                {
                    ControllerManager.AssignVitalist(ControllerManager.PlayerA);
                    SoundManager.Instance.PlaySFXOnline("Menu_VitConnect");
                }
                else if (selectedPlayer == Player.PlayerB)
                {
                    ControllerManager.AssignVitalist(ControllerManager.PlayerB);
                    SoundManager.Instance.PlaySFXOnline("Menu_VitConnect");
                }
                break;
        }

        if (selectedLayer == ButtonLayers.RoleSelect)
        {
            selectables[selectedObjectIndex].transform.GetChild(0).GetComponent<Image>().color = selectColor;
            selectables[selectedObjectIndex].transform.GetChild(1).GetComponent<Image>().color = selectColor;
            selectables[selectedObjectIndex].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = selectColor;

            selectables[selectedObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = selectColor;
        }

        gameObject.transform.GetChild(0).GetComponent<Image>().color = selectColor;
        selectorText.color = selectColor;

        currentMenuState = MenuState.Ready;
    }

    /// <summary>
    /// Select button pressed behaviour
    /// </summary>
    private void SelectPressed()
    {
        if (selectedLayer == ButtonLayers.RoleSelect)
        {
            if (currentMenuState == MenuState.Selecting)
                LockIn(selectedObjectIndex);
            else if (currentMenuState == MenuState.Ready)
                localPlayController.PlayButtonPressed();
        }
        else if (selectedLayer == ButtonLayers.TutorialToggle)
        {
            tutorialToggle.isOn = !tutorialToggle.isOn;
        }
        else if (selectedLayer == ButtonLayers.BackButton)
        {
            backButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// Cancel button pressed behaviour
    /// </summary>
    private void DeselectPressed()
    {
        if (currentMenuState == MenuState.Ready)
        {
            currentMenuState = MenuState.Selecting;
            if (selectedObjectIndex == 0)
                ControllerManager.AssignNavigator(null);
            else if (selectedObjectIndex == 1)
                ControllerManager.AssignVitalist(null);

            if (selectedLayer == ButtonLayers.RoleSelect)
            {
                selectables[selectedObjectIndex].transform.GetChild(0).GetComponent<Image>().color = defaultColor;
                selectables[selectedObjectIndex].transform.GetChild(1).GetComponent<Image>().color = defaultColor;
                selectables[selectedObjectIndex].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = defaultColor;

                selectables[selectedObjectIndex].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = defaultColor;
            }

            gameObject.transform.GetChild(0).GetComponent<Image>().color = defaultColor;
            selectorText.color = defaultColor;
        }
    }

    /// <summary>
    /// Navigate left or right selector
    /// </summary>
    /// <param name="input">Direction to move</param>
    private void MoveLeftOrRight(Vector2 input)
    {
        if (selectedLayer != ButtonLayers.RoleSelect) return;
            if (Mathf.Abs(input.x) <= Mathf.Abs(input.y)) return;
        if (input.x < 0)        // Left input
        {
            selectedObjectIndex--;

            if (selectedObjectIndex < 0)
            {
                selectedObjectIndex = 0;
                return;
            }

            SelectObject(selectables[selectedObjectIndex]);
        }
        else if (input.x > 0)   // Right input
        {
            selectedObjectIndex++;

            if (selectedObjectIndex > selectables.Length - 1)
            {
                selectedObjectIndex = selectables.Length - 1;
                return;
            }

            SelectObject(selectables[selectedObjectIndex]);
        }
    }

    /// <summary>
    /// Navigate up or down selector
    /// </summary>
    /// <param name="input">Direction to move</param>
    private void MoveUpOrDown(Vector2 input)
    {
        if (Mathf.Abs(input.y) <= Mathf.Abs(input.x)) return;
        if (Mathf.RoundToInt(input.y) == 0) return;

        if (input.y > 0) // Moving up 
        {
            switch (selectedLayer)
            {
                case ButtonLayers.RoleSelect:
                    selectedLayer = ButtonLayers.BackButton;
                    SelectObject(backButton.gameObject);
                    break;
                case ButtonLayers.TutorialToggle:
                    selectedLayer = ButtonLayers.RoleSelect;
                    SelectObject(selectables[selectedObjectIndex]);
                    break;
                case ButtonLayers.BackButton:
                    selectedLayer = ButtonLayers.TutorialToggle;
                    SelectObject(tutorialToggle.gameObject);
                    break;
            }
        }
        else if (input.y < 0) // Moving down
        {
            switch (selectedLayer)
            {
                case ButtonLayers.RoleSelect:
                    selectedLayer = ButtonLayers.TutorialToggle;
                    SelectObject(tutorialToggle.gameObject);
                    break;
                case ButtonLayers.TutorialToggle:
                    gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 100);
                    selectedLayer = ButtonLayers.BackButton;
                    SelectObject(backButton.gameObject);
                    break;
                case ButtonLayers.BackButton:
                    selectedLayer = ButtonLayers.RoleSelect;
                    SelectObject(selectables[selectedObjectIndex]);
                    break;
            }
        }
    }
    #endregion
}
