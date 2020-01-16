using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo: MonoBehaviour
{
    [SerializeField] Vector3Int[] firewallLocations;
    [SerializeField] Vector3Int[] defencePlacements;
    [SerializeField] Vector3Int[] terminalLocations;
    [SerializeField] Vector3Int[] portLocations;
    [SerializeField] Vector2Int[] terminalControlAssignments; //x = index of terminal in terminals list, y= index of controlled object in object list
    [SerializeField] Vector3Int[] enemyDeploymentInfo;
    [SerializeField] Vector3Int[] enemy0PatrolRoute;
    [SerializeField] Vector3Int[] enemy1PatrolRoute;
    [SerializeField] Vector3Int[] enemy2PatrolRoute;
    [SerializeField] Vector3Int[] enemy3PatrolRoute;
    [SerializeField] Vector3Int[] enemy4PatrolRoute;
    [SerializeField] Vector3Int[] enemy5PatrolRoute;
    [SerializeField] Vector3Int[] enemy6PatrolRoute;
    [SerializeField] Vector3Int[] enemy7PatrolRoute;
    [SerializeField] Vector3Int[] lootPlacements;

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

    internal Vector3Int[] GetPortLocations()
    {
        return portLocations;
    }

    internal Vector3Int[] GetEnemies()
    {
        return enemyDeploymentInfo;
    }

    internal Vector3Int[] GetPatrolRoute(int enemyIndex)
    {
        switch(enemyIndex)
        {
            case 0: return enemy0PatrolRoute;
            case 1: return enemy1PatrolRoute;
            case 2: return enemy2PatrolRoute;
            case 3: return enemy3PatrolRoute;
            case 4: return enemy4PatrolRoute;
            case 5: return enemy5PatrolRoute;
            case 6: return enemy6PatrolRoute;
            default: return enemy7PatrolRoute;

        }
    }

    internal Vector3Int[] GetDefencePlacements()
    {
        return defencePlacements;
    }

    internal Vector3Int[] GetLootPlacements()
    {
        return lootPlacements;
    }
}