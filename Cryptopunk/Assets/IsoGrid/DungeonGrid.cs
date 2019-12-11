using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField] GameObject gridTile;
    [SerializeField] GameObject ramp;
    [SerializeField] GameObject firewall;
    [SerializeField] GameObject port;
    [SerializeField] GameObject terminal;
    [SerializeField] GameObject[] enemyPrefabs;
    private DungeonTile[][] tileGrid;
    [SerializeField] int numSegments = 3;
    [SerializeField] int searchSize = 8;

    internal void GenerateGrid(int[][] gridHeights)
    {
        tileGrid = new DungeonTile[gridHeights.Length][];
        for(int i=0; i<gridHeights.Length;i++)
        {
            tileGrid[i] = new DungeonTile[gridHeights[i].Length];
            for (int j = 0; j < gridHeights[i].Length; j++)
            {
                tileGrid[i][j] = Instantiate(gridTile, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                tileGrid[i][j].SetHeight(gridHeights[i][j]);
                tileGrid[i][j].xCoord = i;
                tileGrid[i][j].zCoord = j;
            }
        }
    }

    internal void GenerateFirewalls(Vector3Int[] firewallLocations)
    {
        foreach(Vector3Int firewallLocation in firewallLocations)
        {
            DungeonTile tile = tileGrid[firewallLocation.x][firewallLocation.z];
            Firewall newFirewall = Instantiate(firewall, tile.GetOccupyingCoordinates(true),Quaternion.identity).GetComponent<Firewall>();
            DungeonManager.instance.hackableObjects.Add(newFirewall);
            newFirewall.myTile = tile;
        }
    }

    internal void GenerateTerminals(Vector3Int[] terminalLocations)
    {
        foreach (Vector3Int terminalLocation in terminalLocations)
        {
            DungeonTile tile = tileGrid[terminalLocation.x][terminalLocation.z];
            Terminal newTerminal = Instantiate(terminal, tile.GetOccupyingCoordinates(true), Quaternion.identity).GetComponent<Terminal>();
            DungeonManager.instance.terminals.Add(newTerminal);
            newTerminal.myTile = tile;
        }
    }

    internal void AssignControl(Vector2Int[] controlAssignments)
    {
        foreach(Vector2Int controlAssignment in controlAssignments)
        {
            Debug.Log("Assigning control of " + DungeonManager.instance.hackableObjects[controlAssignment.y].name + " to " + DungeonManager.instance.terminals[controlAssignment.x]);
            DungeonManager.instance.terminals[controlAssignment.x].controlledObjects.Add(DungeonManager.instance.hackableObjects[controlAssignment.y]);
        }
    }

    internal void CreateDeploymentZone(Vector3Int startCoords)
    {
        DungeonTile deploymentPoint = tileGrid[startCoords.x][startCoords.z];
        foreach (DungeonTile[] row in tileGrid)
        {
            foreach (DungeonTile tile in row)
            {
                if (IsOpenAndAdjacent(deploymentPoint, tile))
                {
                    tile.isExplored = true;
                }
            }
        }
    }

    internal void GenerateEnemies()
    {
        EnemyProgram newProgram = Instantiate(enemyPrefabs[0]).GetComponent<EnemyProgram>();
        DungeonManager.instance.DeploySecurity(newProgram, tileGrid[1][1]);
        newProgram.waypoints = new List<DungeonTile>();
        newProgram.waypoints.Add(tileGrid[1][1]);
        newProgram.waypoints.Add(tileGrid[1][6]);
        newProgram.waypoints.Add(tileGrid[6][6]);
        newProgram.waypoints.Add(tileGrid[6][1]);

        newProgram = Instantiate(enemyPrefabs[0]).GetComponent<EnemyProgram>();
        DungeonManager.instance.DeploySecurity(newProgram, tileGrid[6][6]);
        newProgram.waypoints = new List<DungeonTile>();
        newProgram.waypoints.Add(tileGrid[6][6]);
        newProgram.waypoints.Add(tileGrid[6][1]);
        newProgram.waypoints.Add(tileGrid[1][1]);
        newProgram.waypoints.Add(tileGrid[1][6]);
    }

    internal void GenerateRamps(List<RampCoordinates> rampCoordinates)
    {
        foreach (RampCoordinates coords in rampCoordinates)
        {
            Vector3Int origin;
            Vector3Int dest;
            if (coords.coord1.y != coords.coord2.y)
            {
                origin = Vector3Int.zero;
                dest = Vector3Int.zero;
                if (coords.coord1.y > coords.coord2.y)
                {
                    dest = coords.coord1;
                    origin = coords.coord2;
                }
                else if (coords.coord2.y > coords.coord1.y)
                {
                    dest = coords.coord2;
                    origin = coords.coord1;
                }
                Ramp newRamp = (Instantiate(ramp).GetComponent<Ramp>());
                newRamp.SetDirection(DetermineRampDirection(origin, dest));
                tileGrid[origin.x][origin.z].SetRamp(newRamp);

            }
        }
    }

    internal void RestrictDeployment(DungeonTile deploymentPoint)
    {
        foreach(DungeonTile[] row in tileGrid)
        {
            foreach(DungeonTile tile in row)
            {
                tile.myCollider.enabled = IsOpenAndAdjacent(deploymentPoint, tile);
            }
        }
    }

    private bool IsOpenAndAdjacent(DungeonTile deploymentPoint, DungeonTile tile)
    {
        return (!tile.isOccupied)&&Mathf.Abs(deploymentPoint.xCoord - tile.xCoord) <= 1 && Mathf.Abs(deploymentPoint.zCoord - tile.zCoord) <= 1;
    }

    private Ramp.Direction DetermineRampDirection(Vector3Int origin, Vector3Int dest)
    {
        if(dest.x - origin.x>0)
        {
            return Ramp.Direction.Right;
        }
        else if(dest.z - origin.z>0)
        {
            return Ramp.Direction.Forward;
        }
        else if (dest.z - origin.z < 0)
        {
            return Ramp.Direction.Back;
        }
        else
        {
            return Ramp.Direction.Left;
        }
    }

    internal DungeonTile GetNewSearchLocation(DungeonTile searcherTile)
    {
        DungeonTile newSearchLocation;
        do
        {
            newSearchLocation = tileGrid[Random.Range(Math.Max(0,searcherTile.xCoord-searchSize), Math.Min(tileGrid.Length,searcherTile.xCoord+searchSize))]
                [Random.Range(Math.Max(0, searcherTile.zCoord - searchSize), Math.Min(tileGrid[0].Length, searcherTile.zCoord + searchSize))];
        }
        while (newSearchLocation.GetHeight() <0);
        Debug.Log(newSearchLocation.GetHeight());
        return newSearchLocation;
    }

    internal DungeonTile GetNearestTileInRange(Program attacker, DungeonTile targetTile, int range, int movesLeft)
    {
        int[][] distances = new int[tileGrid.Length][];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = new int[tileGrid[i].Length];
            for (int j = 0; j < tileGrid[i].Length; j++)
            {
                distances[i][j] = 63;
            }
        }
        SetDistanceRecursive(ref distances, 0, attacker.myTile, searchSize*2, attacker.IsFlying());
        DungeonTile destination= null;
        for(int i=0;i<distances.Length;i++)
        {
            for(int j=0;j<distances[i].Length;j++)
            {
                if(TileDistance(tileGrid[i][j],targetTile)<=range&&(!destination||distances[i][j]<distances[destination.xCoord][destination.zCoord]))
                {
                    destination = tileGrid[i][j];
                }
            }
        }
        return destination;
    }

    internal List<DungeonTile> GetAllSeenTiles(Program program)
    {
        List<DungeonTile> seenTiles = new List<DungeonTile>();
        int minX = Math.Max(0, program.myTile.xCoord - program.sight);
        int minZ = Math.Max(0, program.myTile.zCoord - program.sight);
        int maxX = Math.Min(tileGrid.Length-1, program.myTile.xCoord + program.sight);
        int maxZ = Math.Min(tileGrid[0].Length-1, program.myTile.zCoord + program.sight);
        for(int i=minX;i<=maxX; i++)
        {
            for(int j=minZ; j<=maxZ;j++)
            {
                if (TileDistance(program.myTile, tileGrid[i][j]) <= program.sight && IsInLineOfSight(program, tileGrid[i][j]))
                {
                    seenTiles.Add(tileGrid[i][j]);
                }
            }
        }
        return seenTiles;
    }

    internal void FogOfWarRender(Program[] playerPrograms)
    {
        foreach(DungeonTile[] row in tileGrid)
        {
            foreach(DungeonTile tile in row)
            {
                if(IsSeenByPlayer(tile,playerPrograms))
                {
                    tile.Reveal();
                }
                else
                {
                    tile.Fog();
                }
            }
        }
    }

    private bool IsSeenByPlayer(DungeonTile tile, Program[] playerPrograms)
    {
        bool isSeen = false;
        foreach(Program program in playerPrograms)
        {
            isSeen = isSeen || (TileDistance(tile, program.myTile) <= program.sight&&IsInLineOfSight(program,tile));
        }
        return isSeen;
    }

    internal int TileDistance(DungeonTile tile, DungeonTile myTile)
    {
        return Math.Abs(tile.xCoord - myTile.xCoord) + Math.Abs(tile.zCoord - myTile.zCoord);
    }

    internal bool IsInLineOfSight(Program observer, DungeonTile tile)
    {
        Vector3 direction = (tile.GetOccupyingCoordinates(true) - observer.myTile.GetOccupyingCoordinates(true)).normalized;
        float distance =(tile.GetOccupyingCoordinates(true) -observer.myTile.GetOccupyingCoordinates(true)).magnitude;
        return !Physics.Raycast(observer.myTile.GetOccupyingCoordinates(true), direction, distance, LayerMask.GetMask("Ground"));
    }

    internal List<DungeonTile> FindPath(DungeonTile start, DungeonTile end, int pathLength, bool isFlying)
    {
        int[][] distances = new int[tileGrid.Length][];
        for(int i=0;i<distances.Length;i++)
        {
            distances[i] = new int[tileGrid[i].Length];
            for(int j=0;j<tileGrid[i].Length;j++)
            {
                distances[i][j] = 63;
            }
        }
        SetDistanceRecursive(ref distances,0, end, pathLength, isFlying);
        List<DungeonTile> path = new List<DungeonTile>();
        path.Add(start);
        if(distances[start.xCoord][start.zCoord]<63)
        //only fill out path if destination is reachable
        {
            while(path.Count<=pathLength&&path[path.Count-1]!=end)
            {
                int currentDistance = distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord];
                if (CanMoveToLocation(isFlying, distances, path, 1, 0, currentDistance))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]);
                }
                else if (CanMoveToLocation(isFlying, distances, path, 0, 1, currentDistance))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]);
                }
                else if (CanMoveToLocation(isFlying,distances,path,-1,0,currentDistance))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]);
                }
                else if (CanMoveToLocation(isFlying,distances,path,0,-1,currentDistance))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]);
                }
                else
                {
                    Debug.Log("Rejected all path options");
                    foreach (DungeonTile tile in path)
                    {
                        Debug.Log(tile.xCoord.ToString() + ", " + tile.zCoord.ToString());
                    }
                    break;
                }
            }
        }

        return path;
    }

    private bool CanMoveToLocation(bool isFlying, int[][] distances, List<DungeonTile> path,int deltaX,int deltaZ, int currentDistance)
    {
        return IsValidCoordinates(path[path.Count - 1].xCoord+ deltaX, path[path.Count - 1].zCoord+deltaZ) &&
                            distances[path[path.Count - 1].xCoord + deltaX][path[path.Count - 1].zCoord+deltaZ] == currentDistance - 1 &&
                            (isFlying || IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord+ deltaX][path[path.Count - 1].zCoord+deltaZ]));
    }

    private void SetDistanceRecursive(ref int[][] distances, int currentDistance,DungeonTile tile,int maxDistance,bool isFlying)
    {
        if (distances[tile.xCoord][tile.zCoord] > currentDistance)
        {
            distances[tile.xCoord][tile.zCoord] = currentDistance;
            if (currentDistance < maxDistance)
            {
                if (IsValidCoordinates(tile.xCoord + 1, tile.zCoord) && (isFlying||IsPassable(tileGrid[tile.xCoord + 1][tile.zCoord],tile)))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord + 1][tile.zCoord], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord - 1, tile.zCoord) && (isFlying||IsPassable(tileGrid[tile.xCoord - 1][tile.zCoord],tile)))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord - 1][tile.zCoord], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord, tile.zCoord+1) && (isFlying||IsPassable(tileGrid[tile.xCoord][tile.zCoord+1],tile)))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord][tile.zCoord+1], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord, tile.zCoord-1) && (isFlying||IsPassable(tileGrid[tile.xCoord][tile.zCoord-1],tile)))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord][tile.zCoord-1], maxDistance, isFlying);
                }
            }
        }
    }

    private bool IsPassable(DungeonTile dungeonTile1, DungeonTile dungeonTile2)
        //determines if a transition is possible from tile1 to tile2
    {
        return (!dungeonTile2.isBlocked) && HeightsAreCompatible(dungeonTile1,dungeonTile2);
    }

    private bool HeightsAreCompatible(DungeonTile dungeonTile1, DungeonTile dungeonTile2)
    {
        if(dungeonTile1.ramp)
        {
            if(dungeonTile1.ramp.myDirection==Ramp.Direction.Forward||dungeonTile1.ramp.myDirection==Ramp.Direction.Back)
            {
                if(dungeonTile2.xCoord!=dungeonTile1.xCoord)
                {
                    return false;
                }
            }
            else
            {
                if (dungeonTile2.zCoord != dungeonTile1.zCoord)
                {
                    return false;
                }
            }
        }
        if (dungeonTile2.ramp)
        {
            if (dungeonTile2.ramp.myDirection == Ramp.Direction.Forward || dungeonTile2.ramp.myDirection == Ramp.Direction.Back)
            {
                if (dungeonTile2.xCoord != dungeonTile1.xCoord)
                {
                    return false;
                }
            }
            else
            {
                if (dungeonTile2.zCoord != dungeonTile1.zCoord)
                {
                    return false;
                }
            }
        }
        return (dungeonTile1.GetHeight() == dungeonTile2.GetHeight() || dungeonTile1.ramp || dungeonTile2.ramp);
    }

    private bool IsValidCoordinates(int xCoord, int zCoord)
    {
        if(xCoord<0||xCoord>=tileGrid.Length)
        {
            return false;
        }
        if(zCoord<0||zCoord>=tileGrid[0].Length)
        {
            return false;
        }
        return true;
    }
}
