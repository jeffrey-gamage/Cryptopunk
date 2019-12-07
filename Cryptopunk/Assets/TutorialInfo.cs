using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo: MonoBehaviour
{
    [SerializeField] Vector3Int[] firewallLocations;
    [SerializeField] Vector3Int[] terminalLocations;

    public Vector3Int[] GetFirewallLocations()
    {
        return firewallLocations;
    }

    internal Vector3Int[] GetTerminalInfo()
    {
        return terminalLocations;
    }
}