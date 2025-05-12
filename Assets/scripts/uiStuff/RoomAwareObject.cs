using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAwareObject : MonoBehaviour
{
    public GameObject currentRoom;
    private Vector2Int lastGridPosition;

    public bool isPlayer;
    void Update()
    {
        Vector2Int currentGridPos = WorldMapGenerator.gen.WorldToGridPosition(transform.position);
        
        // Only check when grid position changes
        if (currentGridPos != lastGridPosition)
        {
            currentRoom = WorldMapGenerator.gen.GetRoomAtPosition(transform.position);
            lastGridPosition = currentGridPos;
            
            if (currentRoom != null)
            {
                // Optional: Trigger room entry events
                //currentRoom.GetComponent<RoomBehavior>().playerEntersArea();
            }
        }
    }
}