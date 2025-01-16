using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Nathan Fan
/// Description: Menu controller for the online play role select menu
/// </summary>
public class WaitingNetController : AInstance<WaitingNetController>
{
    [Space(5), Header("Objects")]
    [SerializeField] private GameObject firstMenu;
    [SerializeField] private GameObject secondMenu;
    [SerializeField] private GameObject[] roleObjects;
    [SerializeField] private TMP_Text[] roleSelectors;

    [Space(5), Header("Scripts")]
    [SerializeField] private NetLoadLevel levelLoader;

    [Space(5), Header("Buttons")]
    [SerializeField] private GameObject firstButtonMenuA;
    [SerializeField] private GameObject firstButtonMenuB;

    [Space(5), Header("System Logs")]
    [SerializeField] private TMP_Text systemLogsTextBox;

    private GameObject lastSelectedElement;

    // Ready Up Events
    public NetRoutine readyUpPlayerA;
    public NetRoutine readyUpPlayerB;

    // Swap Events
    public NetRoutine<Boolean> swapPlayerA;
    public NetRoutine<Boolean> swapPlayerB;

    // Confirmation checks
    private bool playerAWantsSwap = false;
    private bool playerBWantsSwap = false;
    private bool playerAReady = false;
    private bool playerBReady = false;


    /// <summary>
    /// Initialize all variables for online play
    /// </summary>
    private void Start()
    {
        firstMenu.SetActive(true);
        secondMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(firstButtonMenuA);

        readyUpPlayerA = new NetRoutine(ReadyUpPlayerA, Ownership.Navigator);   // Host
        readyUpPlayerB = new NetRoutine(ReadyUpPlayerB, Ownership.Vitalist);    // Client
        swapPlayerA = new NetRoutine<Boolean>(PlayerAWantsSwap, Ownership.Navigator);  // Host
        swapPlayerB = new NetRoutine<Boolean>(PlayerBWantsSwap, Ownership.Vitalist);   // Client
    }

    /// <summary>
    /// Main game loop run every frame to ensure a button is always selected for controller support
    /// </summary>
    private void Update()
    {
        if (secondMenu.activeInHierarchy)
            UpdateSystemLogs();

        // Set last selected button
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelectedElement = EventSystem.current.currentSelectedGameObject;
        }
        // If current button is empty, get last or default
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (!lastSelectedElement.activeInHierarchy)
            {
                if (firstMenu.activeInHierarchy)
                    EventSystem.current.SetSelectedGameObject(firstButtonMenuA);
                else if (secondMenu.activeInHierarchy)
                    EventSystem.current.SetSelectedGameObject(firstButtonMenuB);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedElement);
            }
        }
    }

    /// <summary>
    /// Update player role text based on current selection
    /// </summary>
    private void UpdatePlayerRoleDisplays()
    {
        // check player 1 is current player
        if (SteamUser.GetSteamID() == SteamLobby.Player1)
        {
            roleSelectors[0].text = "You";
            roleSelectors[1].text = "Guest";
        }
        // if not, is player 2
        else if (SteamUser.GetSteamID() == SteamLobby.Player2)
        {
            roleSelectors[0].text = "Guest";
            roleSelectors[1].text = "You";
        }
        // Error - You opened the menu without a lobby somehow
        else
        {
            roleSelectors[0].text = "Error";
            roleSelectors[1].text = "Error";
        }
    }

    /// <summary>
    /// When both players join a lobby, initialize the role select
    /// </summary>
    public void BothPlayersInLobby()
    {
        // Swap Menus
        firstMenu.SetActive(false);
        secondMenu.SetActive(true);

        UpdatePlayerRoleDisplays();

        EventSystem.current.SetSelectedGameObject(firstButtonMenuB);
    }

    /// <summary>
    /// If Player A presses the ready button
    /// </summary>
    private void ReadyUpPlayerA()
    {
        playerAReady = true;
        playerAWantsSwap = false;
        playerBWantsSwap = false;

        if (playerAReady && playerBReady)
        {
            levelLoader.StartNetGame();
        }
    }

    /// <summary>
    /// If Player B presses the ready button
    /// </summary>
    private void ReadyUpPlayerB()
    {
        playerBReady = true;
        playerAWantsSwap = false;
        playerBWantsSwap = false;

        if (playerAReady && playerBReady)
        {
            levelLoader.StartNetGame();
        }
    }

    /// <summary>
    /// If player A presses the swap button
    /// </summary>
    /// <param name="playerASwap">If player A has requested a swap or not</param>
    private void PlayerAWantsSwap(Boolean playerASwap)
    {
        playerAWantsSwap = (bool) playerASwap;

        if (playerAWantsSwap && playerBWantsSwap)
        {
            ClearPlayerSwap();
            SteamLobby.SwapPlayers();
        }
    }

    /// <summary>
    /// IF player B presses the swap button
    /// </summary>
    /// <param name="playerBSwap">If player B has requested a swap or not</param>
    private void PlayerBWantsSwap(Boolean playerBSwap)
    {
        playerBWantsSwap = (bool) playerBSwap;

        if (playerAWantsSwap && playerBWantsSwap)
        {
            ClearPlayerSwap();
            SteamLobby.SwapPlayers();
        }
    }

    /// <summary>
    /// Reset player button pressed state
    /// </summary>
    private void ClearPlayerSwap()
    {
        playerAWantsSwap = false;
        playerBWantsSwap = false;
    }

    /// <summary>
    /// Update visual system logs 
    /// </summary>
    private void UpdateSystemLogs()
    {
        systemLogsTextBox.text = string.Empty;
        systemLogsTextBox.text = $"Player1 Swap: {playerAWantsSwap.ToString()}\nPlayer2 Swap: {playerBWantsSwap}";
        systemLogsTextBox.text += $"\nP1: {playerAReady}\nP2: {playerBReady}";
    }

    #region Button Methods
    /// <summary>
    /// Ready button pressed behaviour
    /// </summary>
    public void ReadyButtonPressed()
    {
        if (SteamUser.GetSteamID() == SteamLobby.Player1)
        {
            readyUpPlayerA.Invoke();
        }
        else if (SteamUser.GetSteamID() == SteamLobby.Player2)
        {
            readyUpPlayerB.Invoke();
        }
    }

    /// <summary>
    /// Swap button pressed behaviour
    /// </summary>
    public void SwapButtonPressed()
    {
        if (SteamUser.GetSteamID() == SteamLobby.Player1)
        {
            swapPlayerA.Invoke(!playerAWantsSwap);
        }
        else if (SteamUser.GetSteamID() == SteamLobby.Player2)
        {
            swapPlayerB.Invoke(!playerBWantsSwap);
        }
    }

    /// <summary>
    /// Cancel button pressed behaviour
    /// </summary>
    public void CancelButtonPressed()
    {
        if (secondMenu.activeInHierarchy)
        {
            secondMenu.SetActive(false);
            firstMenu.SetActive(true);
        }
    }
    #endregion
}
