using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Nathan Fan
/// Description: Credits Menu Script, Fades as a transition between credits pages
/// </summary>
public class MenuFade : MonoBehaviour
{
    [Space(5), Header("Script Elements")]
    [SerializeField] private CanvasGroup[] creditsScreens;

    [Space(5), Header("Timing Elements")]
    [SerializeField] private float timeToDisplay = 5f;
    [SerializeField] private float timeToTransition = 1f;

    private Coroutine fadeCoroutine;
    private int currentSelectedScreen = 0;
    private CanvasGroup currentCanvas;
    private CanvasGroup nextCanvas;

    /// <summary>
    /// Start cortoutine 
    /// </summary>
    private void OnEnable()
    {
        ResetScreens();

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine = StartCoroutine(FadeBetweenScreens(0, 1));
    }
    
    /// <summary>
    /// Reset all coroutines
    /// </summary>
    private void OnDisable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = null;
    }

    /// <summary>
    /// Fading Coroutine betweene scenes
    /// </summary>
    /// <param name="currentScreen">Currently displayed scene</param>
    /// <param name="nextScreen">Scene to transition to</param>
    /// <returns></returns>
    private IEnumerator FadeBetweenScreens(int currentScreen, int nextScreen)
    {
        currentSelectedScreen = currentScreen;
        
        CanvasGroup currentCanvas = creditsScreens[currentScreen];
        CanvasGroup nextCanvas = creditsScreens[nextScreen];

        float currentCanvasAlpha = 1;
        float nextCanvasAlpha = 0;
        float timer = 0;

        yield return new WaitForSeconds(timeToDisplay);

        // Loop to transition alpha values
        while (true)
        {
            timer += Time.deltaTime;

            // Lerp the alpha values
            currentCanvasAlpha = Mathf.Lerp(1, 0, timer / timeToTransition);
            nextCanvasAlpha = Mathf.Lerp(0, 1, timer / timeToTransition);

            // Assign values
            currentCanvas.alpha = currentCanvasAlpha;
            nextCanvas.alpha = nextCanvasAlpha;

            if (timer > timeToTransition)
                break;
            
            // Wait until next frame
            yield return null;
        }

        // Full reset on desired outcome just incase
        currentCanvas.alpha = 0;
        nextCanvas.alpha = 1;

        // Prep next transition
        int screenToTranstionTo = (nextScreen + 1) > creditsScreens.Length - 1 ? 0 : nextScreen + 1;
        fadeCoroutine = StartCoroutine(FadeBetweenScreens(nextScreen, screenToTranstionTo));
    }

    /// <summary>
    /// Resets all available screens
    /// </summary>
    private void ResetScreens()
    {
        currentSelectedScreen = 0;

        // Disable all screens
        foreach (var screen in creditsScreens)
        {
            screen.alpha = 0;
        }
        // Enable first screens
        creditsScreens[0].alpha = 1;
    }
}
