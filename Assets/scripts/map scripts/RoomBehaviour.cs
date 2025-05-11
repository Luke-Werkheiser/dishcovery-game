using System.Collections;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [Header("Room Exits")]
    public bool exitNorth;
    public bool exitSouth;
    public bool exitEast;
    public bool exitWest;

public foodItemSpawn foodItem;

    [Header("Optional: Debug Visuals")]
    public bool showGizmos = true;
    public float gizmoLength = 1f;
    [Header("Clearing Exits")]
    public clearingExitManager northExit;
    public clearingExitManager southExit;
    public clearingExitManager eastExit;
    public clearingExitManager westExit;

    [Header("Clearing Destination")]
    public clearingExitManager northDesitation;
    public clearingExitManager southDesitation;
    public clearingExitManager eastDesitation;

    public clearingExitManager westDesitation;

    public Transform[] foodSpawnPoints;

    public float foodRespawnTime;
    void Start()
    {
    }
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.green;
        Vector3 pos = transform.position;

        if (exitNorth)
            Gizmos.DrawLine(pos, pos + Vector3.forward * gizmoLength);
        if (exitSouth)
            Gizmos.DrawLine(pos, pos + Vector3.back * gizmoLength);
        if (exitEast)
            Gizmos.DrawLine(pos, pos + Vector3.right * gizmoLength);
        if (exitWest)
            Gizmos.DrawLine(pos, pos + Vector3.left * gizmoLength);
    }

public void setClearingExitDestination(RoomBehavior targetRoom, Direction direction){
    if (targetRoom == null) return;

    clearingExitManager targetExit = targetRoom.getIncomingDirection(direction); // not opposite!
    clearingExitManager myExit = getExitDirection(direction);
    if(targetExit!=null)
    targetExit.setCanGo(true);
    myExit.setCanGo(true);
    if (targetExit == null || myExit == null) return;

    myExit.connectedPoint = targetExit;
    targetExit.connectedPoint = myExit;

    Debug.Log($"Connected {direction} exits between {name} and {targetRoom.name}");
}    public clearingExitManager getExitDirection(Direction direction){
        return direction switch
        {
            Direction.North => northExit,
            Direction.South => southExit,
            Direction.East => eastExit,
            Direction.West => westExit,
            _ => null
        };

    }
    public clearingExitManager getIncomingDirection(Direction direction){
        return direction switch
        {
            Direction.North => southExit,
            Direction.South => northExit,
            Direction.East => westExit,
            Direction.West => eastExit,
            _ => null
        };

    }
    public Direction getOppositeDirection(Direction d){
        switch (d)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
            default: return Direction.North;
        }
    

    }
    public void spawnFood(){
        Debug.Log("Spawning food");
        int index = 0;
        if(foodItem.foodItem !=null){
            foreach(Transform point in foodSpawnPoints){
                GameObject item = Instantiate(foodItem.foodItem, point.position, point.rotation);
                item.transform.SetParent(point);
                item.GetComponent<foodScript>().index = index;
                item.GetComponent<foodScript>().roomb=this;
                index++;
            }
        }
    }

    public void startRespawnFood(int index){
        StartCoroutine(respawnFood(index));
    }
    private IEnumerator respawnFood(int index){
        yield return new WaitForSeconds(foodRespawnTime);
        Transform point = foodSpawnPoints[index];
                GameObject item = Instantiate(foodItem.foodItem, point.position, point.rotation);
                item.transform.SetParent(point);
                item.GetComponent<foodScript>().index = index;
                item.GetComponent<foodScript>().roomb=this;

    }
}
public enum Direction
{
    North,
    South,
    East,
    West,
}
