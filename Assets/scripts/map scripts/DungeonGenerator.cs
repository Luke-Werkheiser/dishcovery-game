using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public int maxWidth = 5;
    public int maxHeight = 5;
    public int minRooms = 5;
    public int maxRooms = 15;
    public float roomSpacing = 10f;
    [Range(0.1f, 0.9f)] public float connectionChance = 0.5f;

    [Header("Debug Settings")]
    public bool debugMode = true;
    public Color debugRoomColor = Color.green;
    public Color debugConnectionColor = Color.white;
    public float debugLineDuration = 5f;

    [Header("References")]
    public areaScript baseRoom;
    public GameObject roomPrefab;

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
        DisableAllExits(baseRoom); // Start with all exits disabled
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
                ConnectExistingRoom(currentRoom, existingRoom, direction);
            }
            else
            {
                CreateNewRoomConnection(currentRoom, newPos, direction);
            }
        }
    }

    private bool IsPositionValid(Vector2Int position)
    {
        return Mathf.Abs(position.x) < maxWidth && Mathf.Abs(position.y) < maxHeight;
    }

    private void ConnectExistingRoom(areaScript currentRoom, areaScript existingRoom, Direction direction)
    {
        Direction oppositeDir = GetOppositeDirection(direction);
        clearingExitManager destinationExit = existingRoom.GetExitManager(oppositeDir);
        
        currentRoom.ConnectExit(direction, destinationExit);
        existingRoom.ConnectExit(oppositeDir, currentRoom.GetExitManager(direction));
    }

    private void CreateNewRoomConnection(areaScript currentRoom, Vector2Int newPos, Direction direction)
    {
        GameObject newRoomObj = InstantiateRoom(newPos);
        areaScript newRoom = newRoomObj.GetComponent<areaScript>();
        
        DisableAllExits(newRoom); // Start with all exits disabled
        
        generatedRooms.Add(newPos, newRoom);
        generationQueue.Add(newPos);

        Direction oppositeDir = GetOppositeDirection(direction);
        ConnectExistingRoom(currentRoom, newRoom, direction);
    }

    private GameObject InstantiateRoom(Vector2Int position)
    {
        Vector3 spawnPos = new Vector3(position.x * roomSpacing, 0, position.y * roomSpacing);
        return Instantiate(roomPrefab, spawnPos, Quaternion.identity);
    }

    private void DisableAllExits(areaScript room)
    {
        room.DisableExit(Direction.North);
        room.DisableExit(Direction.South);
        room.DisableExit(Direction.East);
        room.DisableExit(Direction.West);
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
                CreateNewRoomConnection(currentRoom, newPos, direction);
            }
        }
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
            Vector3 roomPos = new Vector3(kvp.Key.x * roomSpacing, kvp.Key.y * roomSpacing, 0);
            
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
        if (room.GetDestination(direction) != null)
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
        return new Vector3(connectedPos.x * roomSpacing, connectedPos.y * roomSpacing, 0);
    }
}