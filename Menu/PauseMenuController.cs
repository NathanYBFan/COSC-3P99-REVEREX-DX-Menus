using System.Collections;
using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Author: Nathan Fan
/// Description: Pause menu main controller
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject uiElements;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject buttonHolder;

    [SerializeField] private GameObject initialButton;
    private GameObject lastSelectedButton;

    private Callback<GameOverlayActivated_t> pauseCallback;

    NetRoutine<Ownership> pauseRoutine;
    NetRoutine unpauseRoutine;
    Ownership lastPauser;
    private bool lockLocalPlayer = false;

    private float defaultTimeScale = 1;

    /// <summary>
    /// Initiallization of networked event calls
    /// </summary>
    public void Start()
    {
        pauseRoutine = new NetRoutine<Ownership>(EnablePauseMenu, Ownership.Both);
        unpauseRoutine = new NetRoutine(DisablePauseMenu, Ownership.Both);
        EventManager.GameStart += CreateCallbacks;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Create callbacks/listeners on event call
    /// </summary>
    private void CreateCallbacks()
    {
        ControllerManager.Instance.OnPlayerDisconnect += OpenPauseMenu;
        pauseCallback = Callback<GameOverlayActivated_t>.Create(OpenSteamOverlay);
    }

    /// <summary>
    /// On destroy, reset everything
    /// </summary>
    public void OnDestroy()
    {
        pauseRoutine = null;
        unpauseRoutine = null;
        EventManager.GameStart -= CreateCallbacks;
        if (ControllerManager.Instance != null)
        {
            ControllerManager.Instance.OnPlayerDisconnect -= OpenPauseMenu;
        }
        pauseCallback?.Dispose();
    }

    /// <summary>
    /// Main game loop that runs every frame to ensure that a button is always selected for controller support
    /// </summary>
    private void Update()
    {
        if (SceneManager.GetSceneByName("InGameOptionsMenu").isLoaded) return;

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
    }

    /// <summary>
    /// On enable of the gameobject, initialize the first selected button
    /// </summary>
    private void OnEnable()
    {
        if (GameManager.Instance == null || !GameManager.instance.Navigator.isActiveAndEnabled) return;

        EventSystem.current.SetSelectedGameObject(initialButton);
        lastSelectedButton = initialButton;
    }

    /// <summary>
    /// Open steam overlay 
    /// </summary>
    /// <param name="active">Steam overlay container</param>
    private void OpenSteamOverlay(GameOverlayActivated_t active)
    {
        if (active.m_bActive > 0)
        {
            OpenPauseMenu();
        }
    }

    /// <summary>
    /// Pause the game for both players regardless of who pressed paused
    /// </summary>
    public void OpenPauseMenu()
    {
        if (GameManager.Instance.IsPaused) { return; }
        pauseRoutine.Invoke(Whoami.WhoAmI());
    }

    /// <summary>
    /// Enable pause menu when the game is paused
    /// </summary>
    /// <param name="owner">The player who "owns" the manu: player who pressed the pause button</param>
    private void EnablePauseMenu(Ownership owner)
    {
        SoundManager.Instance.MellowMusic(true);
        lastPauser = owner;
        // Pause game
        Time.timeScale = 0;

        SoundManager.Instance.ToggleAudio(true);

        // Toggle on menu elements
        if (Whoami.AmIOnline())
        {
            displayText.text = GetName() + " Paused";
        }
        else
        {
            displayText.text = "Pause";
        }
        
        // Network the Pause
        uiElements.SetActive(true);
        GameManager.instance.IsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// Close pause menu and reset elements for both players
    /// </summary>
    public void ClosePauseMenu()
    {
        if (!GameManager.Instance.IsPaused || lockLocalPlayer) { return; }
        if (!(GameManager.Instance.Navigator.MovementStateMachine.GetCurrentState() is NavigatorRewindState))
        {
            SoundManager.Instance.ToggleAudio(false);
            SoundManager.Instance.MellowMusic(false);
        }
        if (Whoami.WhoAmI() == lastPauser)
        {
            unpauseRoutine.Invoke();
        }
    }

    /// <summary>
    /// Disable pause menu through net
    /// </summary>
    private void DisablePauseMenu()
    {
        StartCoroutine(DisablePauseCoroutine());
    }

    /// <summary>
    /// Disable pause coroutine for both players after a countdown
    /// </summary>
    /// <returns>Coroutine state</returns>
    private IEnumerator DisablePauseCoroutine()
    {
        if (Whoami.AmIOnline())
        {
            lockLocalPlayer = true;
            float timeToWait = 3.0f;

            displayText.text = GetName() + " Paused: " + timeToWait.ToString();
            while (true)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                timeToWait -= 1.0f;
                displayText.text = GetName() + " Paused: " + timeToWait.ToString();

                if (timeToWait < 0.0f) { break; }
            }
            lockLocalPlayer = false;
        }

        // Unpause game
        Time.timeScale = defaultTimeScale;
        // Toggle off menu elements
        displayText.text = "Play";
        // Network the Unpause
        uiElements.SetActive(false);
        GameManager.instance.IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Open options menu net call
    /// </summary>
    public void OpenOptionsMenu()
    {
        GameManager.Instance.PauseLock = true;
        LevelLoader.LoadAdditively("InGameOptionsMenu");
    }

    /// <summary>
    /// Get name of player who pasued the game as a string
    /// </summary>
    /// <returns>name of player (role name)</returns>
    private string GetName()
    {
        string name = "INVALID";
        switch (lastPauser)
        {
            case Ownership.Navigator:
                name = "Navigator";
                break;
            case Ownership.Vitalist:
                name = "Vitalist";
                break;
        }
        return name;
    }

    /// <summary>
    /// Quit to main menu button pressed behaviour
    /// </summary>
    public void QuitToMenu()
    {
        GameManager.Instance.RestartGame();
    }
}
