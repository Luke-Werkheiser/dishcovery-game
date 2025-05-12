using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Icon Settings")]
    public GameObject mapRoomIconPrefab;
    public Vector2 iconSpacing = new Vector2(50f, 50f); // UI spacing
    public RectTransform mapParent;
    public bool regenerateIcons = false; // Toggle to regenerate icons

    private Dictionary<Vector2Int, MapRoomIcon> icons = new Dictionary<Vector2Int, MapRoomIcon>();
    private Dictionary<Vector2Int, GameObject> lastPlacedRooms; // Cache for regeneration

    public GameObject mapUI;
    public bool mapIsOpen;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // Check if we need to regenerate icons
        if (regenerateIcons)
        {
            RegenerateIcons();
            regenerateIcons = false;
        }

        // Update icon visibility based on player exploration
        foreach (var kvp in icons)
        {
            if (kvp.Value.roomBehavior.playerHasVisited)
                kvp.Value.gameObject.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.M)){
            mapIsOpen=!mapIsOpen;
            mapUI.SetActive(mapIsOpen);
        }
    }
    
    // Called after world generation
    public void GenerateMap(Dictionary<Vector2Int, GameObject> placedRooms)
    {
        // Store reference for potential regeneration
        lastPlacedRooms = new Dictionary<Vector2Int, GameObject>(placedRooms);
        
        // Clear existing icons if any
        ClearAllIcons();

        // Create new icons
        foreach (var kvp in placedRooms)
        {
            Vector2Int gridPos = kvp.Key;
            GameObject roomObj = kvp.Value;
            RoomBehavior behavior = roomObj.GetComponent<RoomBehavior>();

            GameObject iconObj = Instantiate(mapRoomIconPrefab, mapParent);
            iconObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                gridPos.x * iconSpacing.x,
                gridPos.y * iconSpacing.y
            );

            MapRoomIcon icon = iconObj.GetComponent<MapRoomIcon>();
            icon.Setup(behavior.doesHaveNorth, behavior.doesHaveSouth, behavior.doesHaveEast, behavior.doesHaveWest);
            icon.roomBehavior = behavior;
            iconObj.SetActive(behavior.playerHasVisited); // Show if visited

            icons[gridPos] = icon;
        }
    }

    public void RegenerateIcons()
    {
        if (lastPlacedRooms != null && lastPlacedRooms.Count > 0)
        {
            GenerateMap(lastPlacedRooms);
        }
        else
        {
            Debug.LogWarning("No room data available for regeneration");
        }
    }

    public void ClearAllIcons()
    {
        foreach (var kvp in icons)
        {
            if (kvp.Value != null && kvp.Value.gameObject != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        icons.Clear();
    }
}