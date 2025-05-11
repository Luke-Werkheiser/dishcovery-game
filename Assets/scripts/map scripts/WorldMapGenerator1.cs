using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class WorldMapGenerator : MonoBehaviour
{
    public static WorldMapGenerator gen {get; private set;}
    [System.Serializable]
    public class RoomEntry
    {
        public GameObject prefab;
        public bool exitNorth;
        public bool exitSouth;
        public bool exitEast;
        public bool exitWest;
        [Range(0f, 1f)] public float rarity = 1f;

            [Header("foodSettings")]
    public bool canSpawnFood = true;
    [Range(0, 100)]
    public float chanceToHaveFood = 100;
    public spawnType[] types;

    }

    [Header("Room Settings")]
    public List<RoomEntry> roomPrefabs;

    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float roomSpacing = 10f;
    public int maxRooms = 20;

    [Header("Base Room")]
    public GameObject baseRoom;
    public Vector2Int baseRoomGridPosition;
    [Header("Debug")]
    public bool debugRegenerate = false;
    private bool previousDebugState = false;
    private Dictionary<Vector2Int, GameObject> placedRooms = new Dictionary<Vector2Int, GameObject>();
    private int roomCount = 1; // starts with base room
    private System.Random rng = new System.Random();
    public UnityEvent worldGenFinished;
    void Awake()
    {
        gen=this;
    }
    private void Start()
    {
        Invoke(nameof(GenerateWorldMap), .1f);
    }

    private void Update()
    {
        if (debugRegenerate && !previousDebugState)
        {
            RegenerateWorld();
            debugRegenerate = false;
        }

        previousDebugState = debugRegenerate;
    }

    void RegenerateWorld()
    {
        foreach (var kvp in placedRooms)
        {
            if (kvp.Value != baseRoom && kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
rng = new System.Random();
        placedRooms.Clear();
        roomCount = 1;

        GenerateWorldMap();
        Debug.Log("World map regenerated.");
    }    void GenerateWorldMap()
    {
        if (baseRoom == null)
        {
            Debug.LogError("Base room not assigned!");
            return;
        }

        placedRooms.Clear();
        roomCount = 1;

        // Register base room
        placedRooms[baseRoomGridPosition] = baseRoom;
        RoomBehavior baseBehavior = baseRoom.GetComponent<RoomBehavior>();

        Queue<(Vector2Int, RoomBehavior)> frontier = new Queue<(Vector2Int, RoomBehavior)>();
        frontier.Enqueue((baseRoomGridPosition, baseBehavior));

        while (frontier.Count > 0 && roomCount < maxRooms)
        {
            var (currentPos, behavior) = frontier.Dequeue();

            TrySpawnRoom(currentPos, Vector2Int.up, behavior.exitNorth, frontier, Direction.North);
            TrySpawnRoom(currentPos, Vector2Int.down, behavior.exitSouth, frontier, Direction.South);
            TrySpawnRoom(currentPos, Vector2Int.right, behavior.exitEast, frontier, Direction.East);
            TrySpawnRoom(currentPos, Vector2Int.left, behavior.exitWest, frontier, Direction.West);
        }

        // After generation, ensure all adjacent rooms are connected
        ConnectAllRooms();
        worldGenFinished.Invoke();
        Debug.Log($"Finished generating {roomCount} rooms.");
    }


    void TrySpawnRoom(Vector2Int fromPos, Vector2Int direction, bool hasExit, Queue<(Vector2Int, RoomBehavior)> frontier, Direction dirEnum)
    {
        if (!hasExit || roomCount >= maxRooms)
            return;

        Vector2Int newPos = fromPos + direction;

        if (placedRooms.ContainsKey(newPos))
            return;

        if (!IsWithinBounds(newPos) || newPos.y > baseRoomGridPosition.y)
            return;

        List<RoomEntry> candidates = GetCompatibleRooms(direction);
        if (candidates.Count == 0)
            return;

        RoomEntry chosen = ChooseRoomByRarity(candidates);
        if (chosen == null)
            return;

        Vector3 worldPos = new Vector3(newPos.x * roomSpacing, 0f, newPos.y * roomSpacing);
        GameObject newRoom = Instantiate(chosen.prefab, worldPos, Quaternion.identity, transform);

        placedRooms[newPos] = newRoom;
        roomCount++;

        RoomBehavior newBehavior = newRoom.GetComponent<RoomBehavior>();
        newBehavior.exitNorth=chosen.exitNorth;
        newBehavior.exitSouth=chosen.exitSouth;
        newBehavior.exitWest=chosen.exitWest;
        newBehavior.exitEast=chosen.exitEast;
        frontier.Enqueue((newPos, newBehavior));
        if(chosen.canSpawnFood && foodRandomizer.fRandom.doesHaveFood(chosen.chanceToHaveFood)){
            newBehavior.foodItem = foodRandomizer.fRandom.getRandomFood(chosen.types);
            newBehavior.spawnFood();
        }

        // Connect this new room with its origin
        RoomBehavior fromBehavior = placedRooms[fromPos].GetComponent<RoomBehavior>();
        fromBehavior.setClearingExitDestination(newBehavior, dirEnum);
    }

    void ConnectAllRooms()
    {
        foreach (var kvp in placedRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomBehavior behavior = kvp.Value.GetComponent<RoomBehavior>();

            // Check each neighbor for potential connection
            TryConnectNeighbor(pos, behavior, Vector2Int.up, Direction.North);
            TryConnectNeighbor(pos, behavior, Vector2Int.down, Direction.South);
            TryConnectNeighbor(pos, behavior, Vector2Int.right, Direction.East);
            TryConnectNeighbor(pos, behavior, Vector2Int.left, Direction.West);
        }
    }

    void TryConnectNeighbor(Vector2Int pos, RoomBehavior behavior, Vector2Int offset, Direction dirEnum)
    {
        Vector2Int neighborPos = pos + offset;
        if (!placedRooms.ContainsKey(neighborPos))
            return;

        RoomBehavior neighbor = placedRooms[neighborPos].GetComponent<RoomBehavior>();

        // Only connect if both have exits facing each other
        if (HasExitInDirection(behavior, dirEnum) &&
            HasExitInDirection(neighbor, GetOppositeDirection(dirEnum)))
        {
            behavior.setClearingExitDestination(neighbor, dirEnum);
        }
    }

    bool HasExitInDirection(RoomBehavior behavior, Direction dir)
    {
        return dir switch
        {
            Direction.North => behavior.exitNorth,
            Direction.South => behavior.exitSouth,
            Direction.East => behavior.exitEast,
            Direction.West => behavior.exitWest,
            _ => false
        };
    }

    List<RoomEntry> GetCompatibleRooms(Vector2Int direction)
    {
        List<RoomEntry> result = new List<RoomEntry>();

        foreach (RoomEntry room in roomPrefabs)
        {
            if (direction == Vector2Int.up && room.exitSouth) result.Add(room);
            else if (direction == Vector2Int.down && room.exitNorth) result.Add(room);
            else if (direction == Vector2Int.right && room.exitWest) result.Add(room);
            else if (direction == Vector2Int.left && room.exitEast) result.Add(room);
        }

        return result;
    }

    RoomEntry ChooseRoomByRarity(List<RoomEntry> candidates)
    {
        float total = 0f;
        foreach (var room in candidates)
            total += room.rarity;

        float roll = (float)rng.NextDouble() * total;
        float cumulative = 0f;

        foreach (var room in candidates)
        {
            cumulative += room.rarity;
            if (roll <= cumulative)
                return room;
        }

        return null;
    }

    bool IsWithinBounds(Vector2Int pos)
    {
        return Mathf.Abs(pos.x - baseRoomGridPosition.x) <= gridWidth / 2 &&
               Mathf.Abs(pos.y - baseRoomGridPosition.y) <= gridHeight / 2;
    }

    Direction GetOppositeDirection(Direction d)
    {
        return d switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East  => Direction.West,
            Direction.West  => Direction.East,
            _ => Direction.North
        };
    }
}
