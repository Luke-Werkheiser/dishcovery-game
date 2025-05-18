using UnityEngine;

public class FoodObjectSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("The maximum angle the object can sway in degrees.")]
    public float maxSwayAngle = 10f;

    [Tooltip("How quickly the sway effect oscillates.")]
    public float swaySpeed = 5f;

    [Tooltip("How much the player's movement influences the sway.")]
    public float swayInfluence = 0.1f;

    [Tooltip("How quickly the object returns to its neutral rotation.")]
    public float returnSpeed = 2f;

    [Header("Inertia Settings")]
    [Tooltip("How much the object resists changes in its rotation.")]
    public float inertiaDamping = 0.8f;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Quaternion currentVelocity;

    private Transform playerTransform;
    private pickupscript pickupScript;

    void Start()
    {
        initialRotation = transform.localRotation;
        targetRotation = initialRotation;
        currentVelocity = Quaternion.identity;

        // Try to find the pickupscript by going up the hierarchy
        pickupScript = GetComponentInParent<pickupscript>();
        if (pickupScript != null)
        {
            playerTransform = pickupScript.playerObj;
        }
        else
        {
            Debug.LogError("FoodObjectSway: pickupscript not found in parent hierarchy!");
            enabled = false; // Disable the script if the pickup script isn't found
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Calculate sway based on player's horizontal movement
            float swayAmount = playerTransform.InverseTransformDirection(pickupScript.GetComponent<Rigidbody>().velocity).x * swayInfluence;
            float targetSwayRotationAngle = Mathf.Clamp(swayAmount * maxSwayAngle, -maxSwayAngle, maxSwayAngle);
            Quaternion targetSwayRotation = Quaternion.Euler(0f, 0f, targetSwayRotationAngle);
            targetRotation = initialRotation * targetSwayRotation;
        }
        else
        {
            // If no player transform, just return to initial rotation
            targetRotation = initialRotation;
        }

        // Apply inertia
        currentVelocity = Quaternion.Slerp(currentVelocity, Quaternion.identity, Time.deltaTime * inertiaDamping);
        targetRotation *= currentVelocity;

        // Smoothly rotate towards the target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * returnSpeed);
    }

    // Call this method from the pickupscript when the object is picked up
    public void ResetSway()
    {
        initialRotation = transform.localRotation;
        targetRotation = initialRotation;
        currentVelocity = Quaternion.identity;
    }

    // Call this method from the pickupscript to add a rotational force
    public void AddSwayForce(Vector3 force)
    {
        // Convert the force to a torque (simplified)
        Vector3 torque = Vector3.Cross(transform.up, force);
        Quaternion rotationForce = Quaternion.Euler(torque * 10f); // Adjust multiplier for strength
        currentVelocity *= rotationForce;
    }
}