using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

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

    public bool instantRespawn;

    public Transform respawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        if(foodHeight == 0) foodHeight=.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void pickupFood(){
        if(!hasBeenPicked){
            hasBeenPicked = true;
            transform.parent = null;
            if(roomb!=null)
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
        Quaternion startingRotation = transform.rotation;
        Quaternion finalRotation;
        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            if(stackPoint == null) break;
            finalPos = stackPoint.position;
            finalRotation=stackPoint.rotation;
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            transform.rotation=Quaternion.Lerp(startingRotation, finalRotation, (elapsedTime/time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.SetParent(stackPoint);
        //transform.localPosition=Vector3.zero;
        Debug.Log("made it");
        pickupscript.instance.triggerUpdateStack();
    }
    public void triggerRespawn(){
                transform.SetParent(respawnPoint);

        Invoke(nameof(doRespawn), 1f);
    }
    public void doRespawn(){
                transform.position = respawnPoint.position;
        transform.rotation=respawnPoint.rotation;
                transform.SetParent(respawnPoint);

        GetComponent<Collider>().isTrigger=false;
        GetComponent<Rigidbody>().isKinematic=true;

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