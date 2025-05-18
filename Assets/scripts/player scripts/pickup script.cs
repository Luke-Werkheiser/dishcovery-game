using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupscript : MonoBehaviour
{
    public static pickupscript instance { get; private set; }
    
    [Header("References")]
    public Transform holdPoint;
    public LayerMask foodLayer;
    public Transform targetedObj;

    public Transform playerObj;
    
    [Header("Settings")]
    public List<Transform> nearbyObjs = new List<Transform>();
    private List<GameObject> holderObjs = new List<GameObject>();
    private List<GameObject> foodObjs = new List<GameObject>();
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode launchKey = KeyCode.Q;
    public KeyCode suckKey = KeyCode.R;

    public float pickUpCooldownTime = 2f;
    public float dropCooldownTime = 2f;
    public float dropCooldownTimer;
    public float pickupCoolDownTimer;
    public float totalDistance;
    public int stackCount;
    public float moveTime = .5f;
    public static event System.Action<List<foodScript>> OnInventoryChanged;

    [Header("Launch Settings")]
    public Vector3 launchDirection = Vector3.forward; // Now relative to player's forward
    public float launchForce = 10f;
    public float upwardModifier = 0.5f; // How much to adjust upward when launchDirection.y > 0

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (nearbyObjs.Count > 0)
        {
            Transform tempHold = null;
            float tempDistance = Mathf.Infinity;
            foreach (Transform obj in nearbyObjs)
            {
                if (obj == null || (obj.GetComponent<foodScript>() && obj.GetComponent<foodScript>().hasPickUp)) continue;
                if (Vector3.Distance(playerObj.position, obj.position) < tempDistance)
                {
                    tempDistance = Vector3.Distance(playerObj.position, obj.position);
                    tempHold = obj;
                }
            }
            targetedObj = tempHold;
        }

        if (Input.GetKeyDown(pickupKey) && targetedObj != null && dropCooldownTimer < Time.time)
        {
            if (targetedObj.gameObject.GetComponent<foodScript>())
            {
                pickupCoolDownTimer = Time.time + pickUpCooldownTime;
                GameObject holder = new GameObject($"pos {stackCount + 1}");
                holder.transform.position = new Vector3(holdPoint.position.x, holdPoint.position.y + totalDistance, holdPoint.position.z);
                holder.transform.SetParent(holdPoint);
                totalDistance += targetedObj.gameObject.GetComponent<foodScript>().foodHeight;
                targetedObj.gameObject.GetComponent<foodScript>().moveToStack(holder.transform, moveTime);
                holderObjs.Add(holder);
                foodObjs.Add(targetedObj.gameObject);

                targetedObj = null;
            }
        }

        if (Input.GetKeyDown(launchKey) && holderObjs.Count > 0 && foodObjs.Count > 0 && Time.time > pickupCoolDownTimer)
        {
            int foodObjInt = foodObjs.Count - 1;
            int holderObjInt = holderObjs.Count - 1;
            dropCooldownTimer = Time.time + dropCooldownTime;
            if(totalDistance > 15){
                foodObjs[foodObjInt].transform.position= new Vector3(holdPoint.position.x, holdPoint.position.y+15, holdPoint.position.z);
            }
            // Calculate launch direction based on player's forward with modifiers
            Vector3 worldLaunchDir = CalculateLaunchDirection();
            foodObjs[foodObjInt].GetComponent<foodScript>().launchFoodObj(worldLaunchDir, launchForce);
            
            foodObjs[foodObjInt].gameObject.transform.SetParent(null);
            Destroy(holderObjs[holderObjInt], .1f);
            totalDistance -= foodObjs[foodObjInt].GetComponent<foodScript>().foodHeight;
            foodObjs.RemoveAt(foodObjInt);
            OnInventoryChanged?.Invoke(foodObjs.ConvertAll(f => f.GetComponent<foodScript>()));
        }

        if (Input.GetKeyDown(suckKey))
        {
            ItemSucker.itemSucker.tryStartSuck(foodObjs);
        }
    }

    private Vector3 CalculateLaunchDirection()
    {
        // Get player's forward direction (world space)
        Vector3 baseDirection = transform.forward;
        
        // Apply launchDirection modifiers:
        // - x modifies right/left (relative to player)
        // - y modifies up/down (world space)
        // - z modifies forward/backward (relative to player)
        
        Vector3 modifiedDirection = baseDirection * launchDirection.z; // Forward/backward
        modifiedDirection += playerObj.right * launchDirection.x; // Left/right
        modifiedDirection += Vector3.up * launchDirection.y * upwardModifier; // Up/down
        
        return modifiedDirection.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((foodLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (!nearbyObjs.Contains(other.gameObject.transform) || !(other.gameObject.GetComponent<foodScript>() && other.gameObject.GetComponent<foodScript>().hasPickUp))
            {
                nearbyObjs.Add(other.gameObject.transform);
            }
        }
    }

    public void wipeStack()
    {
        // Clear all food objects from stack
        foreach (GameObject foodObj in foodObjs)
        {
            if (foodObj != null)
            {
                foodObj.GetComponent<foodScript>().launchFoodObj(Vector3.down, 1f); // Gently drop items
                foodObj.transform.SetParent(null);
            }
        }

        // Destroy all holder objects
        foreach (GameObject holder in holderObjs)
        {
            if (holder != null)
            {
                Destroy(holder);
            }
        }

        // Clear lists
        foodObjs.Clear();
        holderObjs.Clear();
        nearbyObjs.Clear();
        targetedObj = null;
        totalDistance = 0f;

        // Update UI
        triggerUpdateStack();
    }

    public void triggerUpdateStack()
    {
        OnInventoryChanged?.Invoke(foodObjs.ConvertAll(f => f.GetComponent<foodScript>()));
    }

    private void OnTriggerExit(Collider other)
    {
        if ((foodLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (nearbyObjs.Contains(other.gameObject.transform))
            {
                nearbyObjs.Remove(other.gameObject.transform);
            }
            if (targetedObj == other.gameObject.transform)
            {
                targetedObj = null;
            }
        }
    }
}