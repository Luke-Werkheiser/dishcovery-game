using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class roomClass
{
    public GameObject roomObj;
    public float rarityWeight;
    public bool spawnsFood;
    public bool hasNorthPath = true;
    public bool hasSouthPath = true;
    public bool hasEastPath = true;
    public bool hasWestPath = true;
}

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public int maxWidth = 5;
    public int maxHeight = 5;
    public int minRooms = 5;
    public int maxRooms = 15;
    public float roomSpacing = 10f;
    [Range(0.1f, 0.9f)] public float connectionChance = 0.5f;
    public roomClass[] spawnableRooms;

    [Header("Debug Settings")]
    public bool debugMode = true;
    public Color debugRoomColor = Color.green;
    public Color debugConnectionColor = Color.white;
    public float debugLineDuration = 5f;

    [Header("References")]
    public areaScript baseRoom;

    private Dictionary<Vector2Int, areaScript> generatedRooms = new Dictionary<Vector2Int, areaScript>();
    private List<Vector2Int> generationQueue = new List<Vector2Int>();

    void Start()
    {
        if (debugMode)
        {
            GenerateDungeon();
        }
    }

    public void GenerateDungeon()
    {
        ClearPreviousDungeon();
        InitializeBaseRoom();
        GenerateRoomConnections();
        EnsureMinimumRooms();
        DebugOutput();
    }

    private void ClearPreviousDungeon()
    {
        foreach (var room in FindObjectsOfType<areaScript>())
        {
            if (room != baseRoom)
            {
                Destroy(room.gameObject);
            }
        }
        generatedRooms.Clear();
        generationQueue.Clear();
    }

    private void InitializeBaseRoom()
    {
        baseRoom.gridPosition = Vector2Int.zero;
        generatedRooms.Add(Vector2Int.zero, baseRoom);
        generationQueue.Add(Vector2Int.zero);
    }

    private void GenerateRoomConnections()
    {
        while (generationQueue.Count > 0 && generatedRooms.Count < maxRooms)
        {
            Vector2Int currentPos = generationQueue[0];
            generationQueue.RemoveAt(0);

            TryCreateConnection(currentPos, Direction.North, new Vector2Int(0, -1));
            TryCreateConnection(currentPos, Direction.South, new Vector2Int(0, 1));
            TryCreateConnection(currentPos, Direction.East, new Vector2Int(1, 0));
            TryCreateConnection(currentPos, Direction.West, new Vector2Int(-1, 0));
        }
    }

    private void TryCreateConnection(Vector2Int currentPos, Direction direction, Vector2Int offset)
    {
        if (Random.value > connectionChance) return;

        Vector2Int newPos = currentPos + offset;
        if (IsPositionValid(newPos))
        {
            areaScript currentRoom = generatedRooms[currentPos];
            
            if (generatedRooms.TryGetValue(newPos, out areaScript existingRoom))
            {
                if (CanConnectRooms(currentRoom, direction))
                {
                    ConnectRooms(currentRoom, existingRoom, direction);
                }
            }
            else
            {
                CreateAndConnectNewRoom(currentRoom, newPos, direction);
            }
        }
    }

    private bool CanConnectRooms(areaScript room, Direction direction)
    {
        // Check if the current room has the required exit
        switch (direction)
        {
            case Direction.North: return room.GetExitState(Direction.North);
            case Direction.South: return room.GetExitState(Direction.South);
            case Direction.East: return room.GetExitState(Direction.East);
            case Direction.West: return room.GetExitState(Direction.West);
            default: return false;
        }
    }

    private GameObject GetRandomRoomPrefab(Direction requiredExit)
    {
        // Filter rooms that have the required exit
        var validRooms = spawnableRooms.Where(r => 
            (requiredExit == Direction.North && r.hasSouthPath) ||
            (requiredExit == Direction.South && r.hasNorthPath) ||
            (requiredExit == Direction.East && r.hasWestPath) ||
            (requiredExit == Direction.West && r.hasEastPath)).ToList();

        if (validRooms.Count == 0) return null;

        // Calculate total weight
        float totalWeight = validRooms.Sum(r => r.rarityWeight);
        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;

        foreach (var room in validRooms)
        {
            weightSum += room.rarityWeight;
            if (randomValue <= weightSum)
            {
                return room.roomObj;
            }
        }

        return validRooms[0].roomObj;
    }

    private void CreateAndConnectNewRoom(areaScript currentRoom, Vector2Int newPos, Direction direction)
    {
        // Get required exit direction for the new room
        Direction requiredExit = GetOppositeDirection(direction);
        
        // Get random room that has the required exit
        GameObject roomPrefab = GetRandomRoomPrefab(requiredExit);
        if (roomPrefab == null) return;

        // Create new room
        Vector3 spawnPos = new Vector3(newPos.x * roomSpacing, 0, newPos.y * roomSpacing);
        GameObject newRoomObj = Instantiate(roomPrefab, spawnPos, Quaternion.identity);
        areaScript newRoom = newRoomObj.GetComponent<areaScript>();
        
        // Initialize new room
        newRoom.gridPosition = newPos;
        generatedRooms.Add(newPos, newRoom);
        generationQueue.Add(newPos);
        
        // Connect the rooms
        ConnectRooms(currentRoom, newRoom, direction);
    }

    private void ConnectRooms(areaScript room1, areaScript room2, Direction directionFromRoom1)
    {
        Direction oppositeDir = GetOppositeDirection(directionFromRoom1);
        clearingExitManager exitFromRoom1 = room1.GetExitManager2(directionFromRoom1);
        clearingExitManager exitFromRoom2 = room2.GetExitManager(oppositeDir);
        
        if (exitFromRoom1 != null && exitFromRoom2 != null)
        {
            room1.setClearingDestination(exitFromRoom1, exitFromRoom2);
            room2.setClearingDestination(exitFromRoom2, exitFromRoom1);
            
            room1.SetExitActive(directionFromRoom1, true);
            room2.SetExitActive(oppositeDir, true);
        }
    }

    private void EnsureMinimumRooms()
    {
        while (generatedRooms.Count < minRooms && generationQueue.Count > 0)
        {
            Vector2Int currentPos = generationQueue[0];
            generationQueue.RemoveAt(0);

            ForceCreateConnection(currentPos, Direction.North, new Vector2Int(0, -1));
            ForceCreateConnection(currentPos, Direction.South, new Vector2Int(0, 1));
            ForceCreateConnection(currentPos, Direction.East, new Vector2Int(1, 0));
            ForceCreateConnection(currentPos, Direction.West, new Vector2Int(-1, 0));
        }
    }


    private void ForceCreateConnection(Vector2Int currentPos, Direction direction, Vector2Int offset)
    {
        if (generatedRooms.Count >= minRooms) return;

        Vector2Int newPos = currentPos + offset;
        if (IsPositionValid(newPos))
        {
            areaScript currentRoom = generatedRooms[currentPos];
            
            if (!generatedRooms.ContainsKey(newPos))
            {
                CreateAndConnectNewRoom(currentRoom, newPos, direction);
            }
            else
            {
                areaScript existingRoom = generatedRooms[newPos];
                if (CanConnectRooms(currentRoom, direction))
                {
                    ConnectRooms(currentRoom, existingRoom, direction);
                }
            }
        }
    }

    private bool IsPositionValid(Vector2Int position)
    {
        return Mathf.Abs(position.x) < maxWidth && Mathf.Abs(position.y) < maxHeight;
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
            default: return Direction.North;
        }
    }

    private void DebugOutput()
    {
        if (!debugMode) return;

        Debug.Log($"Generated dungeon with {generatedRooms.Count} rooms");
        DrawDebugConnections();
    }

    private void DrawDebugConnections()
    {
        foreach (var kvp in generatedRooms)
        {
            Vector3 roomPos = new Vector3(kvp.Key.x * roomSpacing, 0, kvp.Key.y * roomSpacing);
            
            // Draw room center marker
            Debug.DrawRay(roomPos, Vector3.up * 0.5f, debugRoomColor, debugLineDuration);
            Debug.DrawRay(roomPos, Vector3.down * 0.5f, debugRoomColor, debugLineDuration);
            Debug.DrawRay(roomPos, Vector3.left * 0.5f, debugRoomColor, debugLineDuration);
            Debug.DrawRay(roomPos, Vector3.right * 0.5f, debugRoomColor, debugLineDuration);

            // Draw connections
            areaScript room = kvp.Value;
            DrawConnectionIfExists(room, roomPos, Direction.North);
            DrawConnectionIfExists(room, roomPos, Direction.South);
            DrawConnectionIfExists(room, roomPos, Direction.East);
            DrawConnectionIfExists(room, roomPos, Direction.West);
        }
    }

    private void DrawConnectionIfExists(areaScript room, Vector3 roomPos, Direction direction)
    {
        if (room.GetExitState(direction))
        {
            Vector3 targetPos = GetConnectedRoomPosition(room, direction);
            Debug.DrawLine(roomPos, targetPos, debugConnectionColor, debugLineDuration);
        }
    }

    private Vector3 GetConnectedRoomPosition(areaScript room, Direction direction)
    {
        Vector2Int connectedPos = room.gridPosition;
        switch (direction)
        {
            case Direction.North: connectedPos += new Vector2Int(0, -1); break;
            case Direction.South: connectedPos += new Vector2Int(0, 1); break;
            case Direction.East: connectedPos += new Vector2Int(1, 0); break;
            case Direction.West: connectedPos += new Vector2Int(-1, 0); break;
        }
        return new Vector3(connectedPos.x * roomSpacing, 0, connectedPos.y * roomSpacing);
    }
}