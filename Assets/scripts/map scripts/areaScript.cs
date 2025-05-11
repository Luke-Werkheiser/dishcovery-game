using UnityEngine;

public class areaScript : MonoBehaviour
{
    public Transform[] spawnPoints;

    [Header("Exit Configuration")]
    public bool hasNorthExit = false;
    public bool hasSouthExit = false;
    public bool hasWestExit = false;
    public bool hasEastExit = false;
    
    [Header("Exit Managers")]
    public clearingExitManager clearingExitUp;    // North exit
    public clearingExitManager clearingExitDown;  // South exit
    public clearingExitManager clearingExitEast;  // East exit
    public clearingExitManager clearingExitWest;  // West exit
    
    [Header("Connected Destinations")]
    public clearingExitManager clearingDestinationUp;    // Where NORTH exit leads
    public clearingExitManager clearingDestinationDown;  // Where SOUTH exit leads
    public clearingExitManager clearingDestinationEast;  // Where EAST exit leads
    public clearingExitManager clearingDestinationWest;  // Where WEST exit leads
    
    public bool playerIsInThisArea;
    public Vector2Int gridPosition;

    public void setClearingDestination(clearingExitManager exitManager, clearingExitManager destination)
    {
        if (exitManager == clearingExitUp)
        {
            clearingDestinationUp = destination;
            clearingExitUp.connectedPoint = destination;
            hasNorthExit = true;
        }
        else if (exitManager == clearingExitDown)
        {
            clearingDestinationDown = destination;
            clearingExitDown.connectedPoint = destination;
            hasSouthExit = true;
        }
        else if (exitManager == clearingExitEast)
        {
            clearingDestinationEast = destination;
            clearingExitEast.connectedPoint = destination;
            hasEastExit = true;
        }
        else if (exitManager == clearingExitWest)
        {
            clearingDestinationWest = destination;
            clearingExitWest.connectedPoint = destination;
            hasWestExit = true;
        }
    }

    public clearingExitManager GetExitManager(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return clearingExitUp;
            case Direction.South: return clearingExitDown;
            case Direction.East: return clearingExitEast;
            case Direction.West: return clearingExitWest;
            default: return null;
        }
    }
    public clearingExitManager GetExitManager2(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return clearingExitDown;
            case Direction.South: return clearingExitUp;
            case Direction.East: return clearingExitEast;
            case Direction.West: return clearingExitWest;
            default: return null;
        }
    }


    public void SetExitActive(Direction direction, bool active)
    {
        switch (direction)
        {
            case Direction.North:
                hasNorthExit = active;
                if (clearingExitUp != null) clearingExitUp.gameObject.SetActive(active);
                break;
            case Direction.South:
                hasSouthExit = active;
                if (clearingExitDown != null) clearingExitDown.gameObject.SetActive(active);
                break;
            case Direction.East:
                hasEastExit = active;
                if (clearingExitEast != null) clearingExitEast.gameObject.SetActive(active);
                break;
            case Direction.West:
                hasWestExit = active;
                if (clearingExitWest != null) clearingExitWest.gameObject.SetActive(active);
                break;
        }
    }

    public bool GetExitState(Direction direction)
    {
        return direction switch
        {
            Direction.North => hasNorthExit,
            Direction.South => hasSouthExit,
            Direction.East => hasEastExit,
            Direction.West => hasWestExit,
            _ => false
        };
    }
}

