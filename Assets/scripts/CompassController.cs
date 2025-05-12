using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro support

public class CompassController : MonoBehaviour
{
    [Header("References")]
    public RectTransform compassNeedle;
    public Transform player;
    public Transform targetObject;
    public RectTransform compassRect;
    public TMP_Text distanceText; // Text to display distance
    
    [Header("Settings")]
    public float edgePadding = 20f;
    public bool smoothRotation = true;
    public float rotationSmoothness = 5f;
    public string distanceFormat = "0.0"; // Format for distance display
    public string distanceSuffix = "m";   // Unit to display after distance

    private Vector2 compassSize;
    private Vector3 flattenedTargetPos;
    private Vector3 flattenedPlayerPos;

    [Header("Target Objs")]
    public GameObject truck;
    public GameObject flag1;
    public GameObject flag2;
    public GameObject flag3;    
    public GameObject flag4;

    private void Start()
    {
        compassSize = compassRect.sizeDelta;
        if(distanceText == null)
        {
            Debug.LogWarning("Distance text reference not set in CompassController");
        }
    }

    private void Update()
    {


        if(Input.GetKeyDown(KeyCode.Alpha1)){
            targetObject=truck.transform;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
            targetObject=flag1.transform;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)){
            targetObject=flag2.transform;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4)){
            targetObject=flag3.transform;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5)){
            targetObject=flag4.transform;
        }

        if (player == null || targetObject == null) return;

        // Flatten positions to 2D plane (XZ to XY)
        flattenedPlayerPos = new Vector3(player.position.x, 0f, player.position.z);
        flattenedTargetPos = new Vector3(targetObject.position.x, 0f, targetObject.position.z);

        // Calculate direction and distance
        Vector3 direction = flattenedTargetPos - flattenedPlayerPos;
        float distance = direction.magnitude;
        
        // Update distance display
        if(distanceText != null)
        {
            distanceText.text = distance.ToString(distanceFormat) + distanceSuffix;
        }

        // Calculate angle and rotate needle
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        
        if (smoothRotation)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -angle);
            compassNeedle.rotation = Quaternion.Lerp(compassNeedle.rotation, targetRotation, 
                                                   rotationSmoothness * Time.deltaTime);
        }
        else
        {
            compassNeedle.rotation = Quaternion.Euler(0f, 0f, -angle);
        }

        UpdateTargetPositionOnCompass(direction);
    }

    private void UpdateTargetPositionOnCompass(Vector3 direction)
    {
        direction.Normalize();
        Vector2 compassDirection = new Vector2(direction.x, direction.z);
        float radius = (Mathf.Min(compassSize.x, compassSize.y) / 2f) - edgePadding;
        Vector2 targetPosition = compassDirection * radius;
    }

    public void SetNewTarget(Transform newTarget)
    {
        targetObject = newTarget;
    }

    // Call this if you need to get the current distance
    public float GetCurrentDistance()
    {
        if(player == null || targetObject == null) return -1f;
        return Vector3.Distance(
            new Vector3(player.position.x, 0f, player.position.z),
            new Vector3(targetObject.position.x, 0f, targetObject.position.z)
        );
    }
}