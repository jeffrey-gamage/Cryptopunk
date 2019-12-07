using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo: MonoBehaviour
{
    [SerializeField] Vector3Int[] firewallLocations;
    [SerializeField] Vector3Int[] terminalLocations;
    [SerializeField] Vector2Int[] terminalControlAssignments; //x = index of terminal in terminals list, y= index of controlled object in object list

    public Vector3Int[] GetFirewallLocations()
    {
        return firewallLocations;
    }

    internal Vector3Int[] GetTerminalInfo()
    {
        return terminalLocations;
    }

    internal Vector2Int[] GetTerminalControlAssignments()
    {
        return terminalControlAssignments;
    }
}