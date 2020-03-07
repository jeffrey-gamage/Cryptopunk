using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalAssignment
{
    internal Vector3Int terminalLocation;
    internal List<Vector3Int> controlLocations;
    private bool terminalLocationIsSet = false;


    public TerminalAssignment()
    {
        controlLocations = new List<Vector3Int>();
    }

    public void AddLocation(Vector3Int newLocation)
    {
        if(!terminalLocationIsSet)
        {
            terminalLocationIsSet = true;
            terminalLocation = newLocation;
        }
        else
        {
            controlLocations.Add(newLocation);
        }
    }

    internal void Translate(Vector3Int translationVector)
    {
        terminalLocation += translationVector;
        if (controlLocations.Count > 0)
        {
            for (int i = 0; i < controlLocations.Count; i++)
            {
                controlLocations[i] += translationVector;
            }
        }
    }
}