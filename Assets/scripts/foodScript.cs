using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodScript : MonoBehaviour
{
    
    public bool hasPickUp = false;
    public float foodHeight;
    Transform stackPoint;
    public foodInfoObj foodInfo;
    public foodInfoObj.foodParts[] ingredients;

    // Gizmo colors
    private Color heightIndicatorColor = Color.green;
    private const float gizmoSphereSize = 0.1f;
    public RoomBehavior roomb;
    public int index;
    public bool hasBeenPicked;
    public bool isBeingCarried;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pickupFood(){
        if(!hasBeenPicked){
            hasBeenPicked = true;
            transform.parent = null;
            roomb.startRespawnFood(index);
        }
    }
    public void createFoodItem(foodInfoObj foodInfo)
    {
        this.foodInfo = foodInfo;
        foodHeight = foodInfo.foodItemHeight;
    }

    public void moveToStack(Transform stackPos, float time)
    {
        stackPoint = stackPos;
        hasPickUp = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        StartCoroutine(SmoothLerp(time));
    }

    public void launchFoodObj(Vector3 launchDir, float launchForce)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
        transform.SetParent(null);
        hasPickUp = false;
        GetComponent<Rigidbody>().AddForce(launchDir * launchForce, ForceMode.Impulse);
    }

    private IEnumerator SmoothLerp(float time)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos;
        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            if(stackPoint == null) break;
            finalPos = stackPoint.position;
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.SetParent(stackPoint);
        pickupscript.instance.triggerUpdateStack();
    }

    // Draw Gizmos when selected in the editor
    private void OnDrawGizmosSelected()
    {

        // Set the gizmo color
        Gizmos.color = heightIndicatorColor;

        // Calculate the top position based on food height
        Vector3 topPosition = transform.position + Vector3.up * foodHeight;

        // Draw a line from the object's position to the top position
        Gizmos.DrawLine(transform.position, topPosition);

        // Draw spheres at the base and top to make it more visible
        Gizmos.DrawSphere(transform.position, gizmoSphereSize);
        Gizmos.DrawSphere(topPosition, gizmoSphereSize);

        // Draw the height value as text
        //UnityEditor.Handles.Label(topPosition + Vector3.up * 0.1f, $"Height: {foodHeight:F2}");
    }
}