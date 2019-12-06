using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo: MonoBehaviour
{
    [SerializeField] Vector3Int[] firewallLocations;

    public Vector3Int[] GetFirewallLocations()
    {
        return firewallLocations;
    }
}