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
    public float stopThreshold = 0.05f;

    [Header("Events")]
    public UnityEvent onTransitionComplete;

    private Coroutine transitionCoroutine;
    private bool waitingToContinue = false;
    private float cachedSpeed = 0f;
    private Vector3 velocity = Vector3.zero;

    private Transform currentPlayer;
    private clearingExitManager currentExitManager;
    private Vector3 currentMoveDir;
    private Vector3 currentLookDir;

    public void StartTransition(Vector2 direction, clearingExitManager exitManager, Transform player)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        waitingToContinue = false;
        cachedSpeed = 0f;
        velocity = Vector3.zero;

        currentPlayer = player;
        currentExitManager = exitManager;

        // Clamp and normalize movement direction
        direction.x = Mathf.Clamp(direction.x, -1f, 1f);
        direction.y = Mathf.Clamp(direction.y, -1f, 1f);
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

        // Smoothly rotate player toward look direction
        Quaternion targetRotation = Quaternion.LookRotation(currentLookDir);
        while (Quaternion.Angle(currentPlayer.rotation, targetRotation) > 0.5f)
        {
            currentPlayer.rotation = Quaternion.RotateTowards(
                currentPlayer.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Acceleration phase - move until close enough to decelerate
        while (true)
        {
            float distance = Vector3.Distance(
                new Vector3(currentPlayer.position.x, 0, currentPlayer.position.z),
                new Vector3(currentExitManager.transform.position.x, 0, currentExitManager.transform.position.z)
            );

            if (distance >= distanceBeforeDeceleration)
            {
                cachedSpeed = Mathf.Min(cachedSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
                currentPlayer.position += currentMoveDir * cachedSpeed * Time.fixedDeltaTime;
            }
            else
            {
                break; // Enter deceleration phase
            }

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => !waitingToContinue);

        // Deceleration / smoothing to final exit point
        Vector3 target = currentExitManager.exitPoint.position;
        target.y = currentPlayer.position.y;

        while (Vector3.Distance(currentPlayer.position, target) > stopThreshold)
        {
            currentPlayer.position = Vector3.SmoothDamp(
                currentPlayer.position,
                target,
                ref velocity,
                0.3f, // smoothing time
                maxSpeed,
                Time.fixedDeltaTime
            );
            yield return new WaitForFixedUpdate();
        }

        // Snap to final position
        currentPlayer.position = target;
        cachedSpeed = 0f;

        onTransitionComplete?.Invoke();
    }
}
