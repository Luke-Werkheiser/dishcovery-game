using UnityEngine;

public class MapRoomIcon : MonoBehaviour
{
    [Header("Exits")]
    public GameObject northExit;
    public GameObject southExit;
    public GameObject eastExit;
    public GameObject westExit;
    [Header("Icons")]
    public GameObject playerIcon;
    public GameObject flag1, flag2, flag3, flag4;

    public GameObject truckIcon;
    [HideInInspector]
    public RoomBehavior roomBehavior;

    public void Setup(bool hasNorth, bool hasSouth, bool hasEast, bool hasWest)
    {
        if (northExit != null) northExit.SetActive(hasNorth);
        if (southExit != null) southExit.SetActive(hasSouth);
        if (eastExit  != null) eastExit.SetActive(hasEast);
        if (westExit  != null) westExit.SetActive(hasWest);
    }
    void Update()
    {
        if(playerIcon!=null){
            playerIcon.SetActive(roomBehavior.playerIsInArea);
        }
        if(flag1!=null){
            flag1.SetActive(roomBehavior.hasFlag1);
        }
                if(flag2!=null){
            flag2.SetActive(roomBehavior.hasFlag2);
        }
        if(flag3!=null){
            flag3.SetActive(roomBehavior.hasFlag3);
        }
        if(flag4!=null){
            flag4.SetActive(roomBehavior.hasFlag4);
        }

    }
}
