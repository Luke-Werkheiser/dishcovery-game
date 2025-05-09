using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class pickupscript : MonoBehaviour
{
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

    public float totalDistance;
    public int stackCount;
    public float moveTime = .5f;


    public Vector3 launchDirection;
    public float launchForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(nearbyObjs.Count>0){
            Transform tempHold = null;
            float tempDistance = Mathf.Infinity;
            foreach(Transform obj in nearbyObjs){
                if(obj==null || (obj.GetComponent<foodScript>() && obj.GetComponent<foodScript>().hasPickUp)) continue;
                if(Vector3.Distance(playerObj.position, obj.position) < tempDistance){
                    tempDistance = Vector3.Distance(playerObj.position, obj.position);
                    tempHold=obj;
                }
            }
            targetedObj = tempHold;
        }
        if(Input.GetKeyDown(pickupKey) && targetedObj!=null){
            if(targetedObj.gameObject.GetComponent<foodScript>()){
                GameObject holder = new GameObject($"pos {stackCount+1}");
                holder.transform.position =new Vector3(holdPoint.position.x, holdPoint.position.y + totalDistance, holdPoint.position.z);
                holder.transform.SetParent(holdPoint);
                totalDistance +=targetedObj.gameObject.GetComponent<foodScript>().foodHeight;
                targetedObj.gameObject.GetComponent<foodScript>().moveToStack(holder.transform, moveTime);
                holderObjs.Add(holder);
                                foodObjs.Add(targetedObj.gameObject);

                targetedObj=null;
            }
        }
        if(Input.GetKeyDown(launchKey) && holderObjs.Count>0 && foodObjs.Count >0){
            int foodObjInt = foodObjs.Count-1;
            int holderObjInt = holderObjs.Count-1;
            foodObjs[foodObjInt].GetComponent<foodScript>().launchFoodObj(launchDirection, launchForce);
            foodObjs[foodObjInt].gameObject.transform.SetParent(null);
            Destroy(holderObjs[holderObjInt], .1f);
            totalDistance -= foodObjs[foodObjInt].GetComponent<foodScript>().foodHeight;
            foodObjs.RemoveAt(foodObjInt);

        }
        if(Input.GetKeyDown(suckKey)){
            ItemSucker.itemSucker.tryStartSuck(foodObjs);
        }
    }
     private void OnTriggerEnter(Collider other)
    {
        if (( foodLayer & (1 << other.gameObject.layer)) != 0)
        {
            if(!nearbyObjs.Contains(other.gameObject.transform) || !(other.gameObject.GetComponent<foodScript>() && other.gameObject.GetComponent<foodScript>().hasPickUp)){
                nearbyObjs.Add(other.gameObject.transform);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (( foodLayer & (1 << other.gameObject.layer)) != 0)
        {
            if(nearbyObjs.Contains(other.gameObject.transform)){
                nearbyObjs.Remove(other.gameObject.transform);
            }
            if(targetedObj == other.gameObject.transform){
                targetedObj=null;
            }
        }

    }
}
