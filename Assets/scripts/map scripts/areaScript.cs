using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areaScript : MonoBehaviour
{
    public Transform[] spawnPoints;
    
    [Header("Exit Configuration")]
    public bool hasNorthExit = true;
    public bool hasSouthExit = true;
    public bool hasWestExit = true;
    public bool hasEastExit = true;
    
    [Header("Exit Managers")]
    public clearingExitManager clearingExitUp;    // North
    public clearingExitManager clearingExitDown;  // South
    public clearingExitManager clearingExitEast;  // East
    public clearingExitManager clearingExitWest;  // West
    
    [Header("Connected Exit Destinations")]
    public clearingExitManager clearingDestinationUp;    // Where NORTH exit leads to
    public clearingExitManager clearingDestinationDown;  // Where SOUTH exit leads to
    public clearingExitManager clearingDestinationEast;  // Where EAST exit leads to
    public clearingExitManager clearingDestinationWest;  // Where WEST exit leads to
    
    public bool playerIsInThisArea;
    
    [Header("Position in Grid")]
    public Vector2Int gridPosition;

    public void ConnectExit(Direction direction, clearingExitManager destinationExit)
    {
        switch (direction)
        {
            case Direction.North:
                clearingDestinationUp = destinationExit;
                if (clearingExitUp != null)
                {
                    clearingExitUp.connectedPoint = destinationExit;
                    hasNorthExit = true;
                }
                break;
                
            case Direction.South:
                clearingDestinationDown = destinationExit;
                if (clearingExitDown != null)
                {
                    clearingExitDown.connectedPoint = destinationExit;
                    hasSouthExit = true;
                }
                break;
                
            case Direction.East:
                clearingDestinationEast = destinationExit;
                if (clearingExitEast != null)
                {
                    clearingExitEast.connectedPoint = destinationExit;
                    hasEastExit = true;
                }
                break;
                
            case Direction.West:
                clearingDestinationWest = destinationExit;
                if (clearingExitWest != null)
                {
                    clearingExitWest.connectedPoint = destinationExit;
                    hasWestExit = true;
                }
                break;
        }
    }
    
    public void DisableExit(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                hasNorthExit = false;
                if (clearingExitUp != null) clearingExitUp.gameObject.SetActive(false);
                clearingDestinationUp = null;
                break;
                
            case Direction.South:
                hasSouthExit = false;
                if (clearingExitDown != null) clearingExitDown.gameObject.SetActive(false);
                clearingDestinationDown = null;
                break;
                
            case Direction.East:
                hasEastExit = false;
                if (clearingExitEast != null) clearingExitEast.gameObject.SetActive(false);
                clearingDestinationEast = null;
                break;
                
            case Direction.West:
                hasWestExit = false;
                if (clearingExitWest != null) clearingExitWest.gameObject.SetActive(false);
                clearingDestinationWest = null;
                break;
        }
    }
    
    // Helper method to get exit manager for a direction
    public clearingExitManager GetExitManager(Direction direction)
    {
        return direction switch
        {
            Direction.North => clearingExitUp,
            Direction.South => clearingExitDown,
            Direction.East => clearingExitEast,
            Direction.West => clearingExitWest,
            _ => null
        };
    }
    
    // Helper method to get destination for a direction
    public clearingExitManager GetDestination(Direction direction)
    {
        return direction switch
        {
            Direction.North => clearingDestinationUp,
            Direction.South => clearingDestinationDown,
            Direction.East => clearingDestinationEast,
            Direction.West => clearingDestinationWest,
            _ => null
        };
    }
}

public enum Direction
{
    North,
    South,
    East,
    West
}