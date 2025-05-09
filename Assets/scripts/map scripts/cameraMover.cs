using UnityEngine;
using UnityEngine.Events;
public class cameraMover : MonoBehaviour
{
    [Header("Target to Move")]
    public Transform targetObject;
    public UnityEvent finishedMovingCamera;
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public bool playerIsMoving =false;
    [Header("Debug/Test Settings")]
    public bool debugMove = false;
    public Transform debugStartPos;
    public Transform debugEndPos;

    private Coroutine moveCoroutine;
    void Update()
    {
        // Test movement via debug bool
        if (debugMove)
        {
            debugMove = false;
            MoveFromTo(debugStartPos, debugEndPos);
        }
    }

    public void MoveFromTo(Transform startPos, Transform endPos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        Debug.Log("Moving camera");
        moveCoroutine = StartCoroutine(MoveRoutine(startPos.position, endPos.position));
    }

    private System.Collections.IEnumerator MoveRoutine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float currentSpeed = 0f;
        float traveled = 0f;
        Vector3 direction = (end - start).normalized;

        targetObject.position = start;

        while (traveled < distance)
        {
            // Calculate how much we have left and determine if we should accelerate or decelerate
            float remaining = distance - traveled;
            float desiredSpeed = Mathf.Min(maxSpeed, Mathf.Sqrt(2 * deceleration * remaining));

            if (currentSpeed < desiredSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, desiredSpeed, maxSpeed);
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, desiredSpeed);
            }

            float step = currentSpeed * Time.deltaTime;
            traveled += step;
            targetObject.position += direction * step;

            yield return null;
        }

        targetObject.position = end; // Snap to exact position at the end
        finishedMovingCamera.Invoke();
        yield return null;
    }
}
