using UnityEngine;
using UnityEngine.Events;

public class PlayerClearingTransition : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 8f;
    public float deceleration = 8f;
    public float distanceBeforeDeceleration = 10f;
    public float rotationSpeed = 360f; // degrees per second

    [Header("Events")]
    public UnityEvent onTransitionComplete;

    private Coroutine transitionCoroutine;
    private bool waitingToContinue = false;
    private float cachedSpeed = 0f;

    private Transform currentPlayer;
    private clearingExitManager currentExitManager;
    private Vector3 currentMoveDir;
    private Vector3 currentLookDir;
    public int attemptTimes = 500;
    public void StartTransition(Vector2 direction, clearingExitManager exitManager, Transform player)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        waitingToContinue = false;
        cachedSpeed = 0f;

        currentPlayer = player;
        currentExitManager = exitManager;

        // Clamp direction
        direction.x = Mathf.Clamp(direction.x, -1, 1);
        direction.y = Mathf.Clamp(direction.y, -1, 1);
        currentMoveDir = new Vector3(direction.x, 0f, direction.y).normalized;
        currentLookDir = new Vector3(direction.y, 0f, direction.x).normalized;


        transitionCoroutine = StartCoroutine(TransitionRoutine());
    }

    public void ContinueMovement()
    {
        if (!waitingToContinue) return;

        waitingToContinue = false;
    }

    private System.Collections.IEnumerator TransitionRoutine()
    {
        waitingToContinue = true;
        int attempts = 0;
        // Smoothly rotate player toward direction
        Quaternion targetRotation = Quaternion.LookRotation(currentLookDir);
        while (false && Quaternion.Angle(currentPlayer.rotation, targetRotation) > 0.5f)
        {
            currentPlayer.rotation = Quaternion.RotateTowards(currentPlayer.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Accelerate until distanceBeforeDeceleration is reached
        while (attempts <attemptTimes)
        {
            float distanceFromOrigin = Vector3.Distance(new Vector3(currentPlayer.position.x, 0, currentPlayer.position.z),
                                                        new Vector3(currentExitManager.transform.position.x, 0, currentExitManager.transform.position.z));

            if (distanceFromOrigin >= distanceBeforeDeceleration)
            {
                cachedSpeed = Mathf.Min(cachedSpeed + acceleration * Time.deltaTime, maxSpeed);
                currentPlayer.position += currentMoveDir * cachedSpeed * Time.deltaTime;
            }
            else
            {
                // Stop movement and wait for ContinueMovement
                break;
            }
            attempts ++;
            yield return new WaitForEndOfFrame();
           // yield return null;
        }

        yield return new WaitUntil(() => !waitingToContinue);

        // Deceleration phase after continue
    Vector3 target = currentExitManager.exitPoint.position;
    target.y = currentPlayer.position.y;

    while (Vector3.Distance(currentPlayer.position, target) > 0.05f)
    {
        Vector3 moveDir = (target - currentPlayer.position).normalized;
        currentPlayer.position += moveDir * maxSpeed * Time.deltaTime;
        yield return null;
    }

    // Snap exactly to the point
    currentPlayer.position = target;
    cachedSpeed = 0f;
        onTransitionComplete?.Invoke();
    }
}
