using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Nathan Fan
/// Description: Lerp behaviour of menu items between 2 points
/// </summary>
public class ItemLerpMove : MonoBehaviour
{
    [SerializeField] private float timePerLoop;
    [SerializeField] private Vector3 positionToMoveTo;
    [SerializeField] private GameObject objectToMove;

    [SerializeField] private Coroutine moveCoroutine;
    private Vector3 startingPos;

    /// <summary>
    /// Initial location saving on startup
    /// </summary>
    private void OnEnable()
    {
        startingPos = objectToMove.transform.localPosition;
        moveCoroutine = StartCoroutine(MoveLeft());
    }

    /// <summary>
    /// Reset everything on disable
    /// </summary>
    private void OnDisable()
    {
        StopCoroutine(moveCoroutine);
        objectToMove.transform.localPosition = startingPos;
    }

    /// <summary>
    /// Move left lerp 
    /// </summary>
    /// <returns>Status of coroutine</returns>
    public IEnumerator MoveLeft()
    {
        float timer = 0;
        Vector3 endPos = startingPos + positionToMoveTo;

        while (true)
        {
            timer += Time.deltaTime;

            float interpolationRatio = timer / timePerLoop;
            Vector3 interpolatedPosition = Vector3.Lerp(startingPos, endPos, interpolationRatio);

            objectToMove.transform.localPosition = interpolatedPosition;

            if (interpolatedPosition == endPos)
                break;

            yield return null;
        }

        // Start the move right lerp
        moveCoroutine = StartCoroutine(MoveRight());
    }

    /// <summary>
    /// Move right lerp
    /// </summary>
    /// <returns>Status of coroutine</returns>
    public IEnumerator MoveRight() 
    {
        float timer = 0;

        Vector3 startPos = startingPos + positionToMoveTo;
        Vector3 endPos = startingPos;

        while (true)
        {
            timer += Time.deltaTime;

            float interpolationRatio = timer / timePerLoop;
            Vector3 interpolatedPosition = Vector3.Lerp(startPos, endPos, interpolationRatio);

            objectToMove.transform.localPosition = interpolatedPosition;

            if (interpolatedPosition == endPos)
                break;

            yield return null;
        }

        moveCoroutine = StartCoroutine(MoveLeft());
    }
}
