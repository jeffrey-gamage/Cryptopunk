using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField] readonly int STARTING_EXPLORE_DISTANCE = 5;
    [SerializeField] GameObject gridTile;
    [SerializeField] GameObject ramp;
    [SerializeField] GameObject firewall;
    [SerializeField] GameObject port;
    [SerializeField] GameObject terminal;
    [SerializeField] GameObject bridgeSwitch;
    [SerializeField] GameObject switchTileOn;
    [SerializeField] GameObject switchTileOff;
    [SerializeField] internal GameObject[] enemyPrefabs;
    [SerializeField] GameObject[] defencePrefabs;
    [SerializeField] GameObject hubPrefab;
    [SerializeField] GameObject lootPrefab;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject deploymentZone;
    private DungeonTile[][] tileGrid;
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

    internal bool CanDeploySecurityHere(int xCoord, int zCoord)
    {
        if(IsValidCoordinates(xCoord,zCoord))
        {
            return (!tileGrid[xCoord][zCoord].isOccupied) && ((tileGrid[xCoord][zCoord].GetHeight() >= 0 && !tileGrid[xCoord][zCoord].isBlocked));
        }
        return false;
    }

    internal void GenerateFirewalls(Vector3Int[] firewallLocations)
    {
        foreach(Vector3Int firewallLocation in firewallLocations)
        {
            if (IsValidCoordinates(firewallLocation.x, firewallLocation.z))
            {
                DungeonTile tile = tileGrid[firewallLocation.x][firewallLocation.z];
                Firewall newFirewall = Instantiate(firewall, tile.GetOccupyingCoordinates(true,true), Quaternion.identity).GetComponent<Firewall>();
                DungeonManager.instance.hackableObjects.Add(newFirewall);
                newFirewall.myTile = tile;
            }
            else
            {
                Debug.LogWarning("Attempeted to place firewall at invalid coordinates " + firewallLocation.ToString());
            }
        }
    }

    internal void GenerateSwitchTilesOff(List<Vector3Int> switchTilesOff)
    {
        foreach(Vector3Int switchTileCoords in switchTilesOff)
        {
            Destroy(tileGrid[switchTileCoords.x][switchTileCoords.z].gameObject);
            tileGrid[switchTileCoords.x][switchTileCoords.z] = Instantiate(switchTileOff, Vector3.right * switchTileCoords.x + Vector3.forward * switchTileCoords.z, Quaternion.identity).GetComponent<SwitchTile>();
            tileGrid[switchTileCoords.x][switchTileCoords.z].SetHeight(switchTileCoords.y);
            tileGrid[switchTileCoords.x][switchTileCoords.z].xCoord = switchTileCoords.x;
            tileGrid[switchTileCoords.x][switchTileCoords.z].zCoord = switchTileCoords.z;
        }
    }

    internal void GenerateSwitchTilesOn(List<Vector3Int> switchTilesOn)
    {

        foreach (Vector3Int switchTileCoords in switchTilesOn)
        {
            Destroy(tileGrid[switchTileCoords.x][switchTileCoords.z].gameObject);
            tileGrid[switchTileCoords.x][switchTileCoords.z] = Instantiate(switchTileOn, Vector3.right * switchTileCoords.x + Vector3.forward * switchTileCoords.z, Quaternion.identity).GetComponent<SwitchTile>();
            tileGrid[switchTileCoords.x][switchTileCoords.z].SetHeight(switchTileCoords.y);
            tileGrid[switchTileCoords.x][switchTileCoords.z].xCoord = switchTileCoords.x;
            tileGrid[switchTileCoords.x][switchTileCoords.z].zCoord = switchTileCoords.z;
        }
    }

    internal void GenerateSwitches(List<Vector3Int> switchLocations)
    {
        foreach (Vector3Int switchLocation in switchLocations)
        {
            if (IsValidCoordinates(switchLocation.x, switchLocation.z))
            {
                DungeonTile tile = tileGrid[switchLocation.x][switchLocation.z];
                SwitchBridge newSwitch = Instantiate(bridgeSwitch, tile.GetOccupyingCoordinates(true, true), Quaternion.identity).GetComponent<SwitchBridge>();
                DungeonManager.instance.hackableObjects.Add(newSwitch);
                DungeonManager.instance.switchBridges.Add(newSwitch);
                newSwitch.myTile = tile;
            }
            else
            {
                Debug.LogWarning("Attempeted to place switch at invalid coordinates " + switchLocation.ToString());
            }
        }
    }

    internal int SelectEnemy(int difficultyBudget)
    {
        int maxIndex = 1;
        for(int i=0;i<enemyPrefabs.Length;i++)
        {
            if(difficultyBudget>=enemyPrefabs[i].GetComponent<EnemyProgram>().difficultyRating)
            {
                maxIndex = i;
            }
        }
        return UnityEngine.Random.Range(0, maxIndex);
    }

    internal void AssignPatrolRoutes(ref List<EnemyProgram> enemiesInRoom, List<List<Vector3Int>> patrolRoutes)
    {
        foreach(List<Vector3Int> patrolRoute in patrolRoutes)
        {
            foreach(EnemyProgram enemy in enemiesInRoom)
            {
                if(enemy.myTile.xCoord==patrolRoute[0].x&&enemy.myTile.zCoord==patrolRoute[0].z)
                {
                    enemy.SetWaypoints(patrolRoute.ToArray());
                }
            }
        }
    }

    internal int GetWidth()
    {
        return tileGrid.Length;
    }

    internal int GetHeight()
    {
        return tileGrid[0].Length;
    }

    internal Vector3 GetCentrePoint()
    {
        int centreX = tileGrid.Length / 2;
        int centreZ = tileGrid[0].Length / 2;
        return tileGrid[centreX][centreZ].transform.position;
    }

    internal void GenerateTerminals(Vector3Int[] terminalLocations)
    {
        foreach (Vector3Int terminalLocation in terminalLocations)
        {
            DungeonTile tile = tileGrid[terminalLocation.x][terminalLocation.z];
            Terminal newTerminal = Instantiate(terminal, tile.GetOccupyingCoordinates(false,true), Quaternion.identity).GetComponent<Terminal>();
            DungeonManager.instance.terminals.Add(newTerminal);
            newTerminal.myTile = tile;
        }
    }

    internal int GetEnemyRating(int enemyIndex)
    {
        return enemyPrefabs[enemyIndex].GetComponent<EnemyProgram>().difficultyRating;
    }

    internal void GenerateSecurityHubs(Vector3Int[] hubPlacements)
    {
        foreach (Vector3Int hubLocation in hubPlacements)
        {
            DungeonTile tile = tileGrid[hubLocation.x][hubLocation.z];
            SecurityNode newNode = Instantiate(hubPrefab, tile.GetOccupyingCoordinates(false,true), Quaternion.identity).GetComponent<SecurityNode>();
            DungeonManager.instance.securityNodes.Add(newNode);
            newNode.myTile = tile;
        }
    }

    internal void GenerateDefenses(Vector3Int[] defenceArray)
    {
        foreach (Vector3Int defenceInfo in defenceArray)
        {
            DungeonTile tile = tileGrid[defenceInfo.x][defenceInfo.z];
            GameObject newDefense =Instantiate(defencePrefabs[defenceInfo.y]);
            DungeonManager.instance.hackableObjects.Add(newDefense.GetComponent<Hackable>());
            DungeonManager.instance.DeploySecurity(newDefense.GetComponent<EnemyProgram>(), tile);
        }
    }

    internal void PlaceLoot(Vector3Int[] lootInfo)
    {
        foreach (Vector3Int lootPos in lootInfo)
        {
            Loot newLoot = Instantiate(lootPrefab, tileGrid[lootPos.x][lootPos.z].GetOccupyingCoordinates(true,true), Quaternion.identity).GetComponent<Loot>();
            newLoot.setContents(lootPos.y);
            tileGrid[lootPos.x][lootPos.z].loot = newLoot;
        }
    }

    internal void PlaceObjective(Vector3Int missionObj)
    {
        Loot newLoot = Instantiate(objectivePrefab, tileGrid[missionObj.x][missionObj.z].GetOccupyingCoordinates(true,true), Quaternion.identity).GetComponent<Loot>();
        newLoot.setContents(missionObj.y);
        tileGrid[missionObj.x][missionObj.z].loot = newLoot;
    }

    internal void AssignTerminalControl(Vector3Int[] controlAssignments)
    {
        foreach(Vector3Int controlAssignment in controlAssignments)
        {
            if (controlAssignment.y < 0)//signal for room camera
            {
                Room.RoomBoundaries boundaries = DungeonManager.instance.generator.rooms[controlAssignment.z].CalculateRoomBoundaries();
                DungeonManager.instance.terminals[controlAssignment.x].minCamX = boundaries.minX;
                DungeonManager.instance.terminals[controlAssignment.x].minCamZ = boundaries.minZ;
                DungeonManager.instance.terminals[controlAssignment.x].maxCamX = boundaries.maxX;
                DungeonManager.instance.terminals[controlAssignment.x].maxCamZ = boundaries.maxZ;
            }
            else
            {
                DungeonManager.instance.terminals[controlAssignment.x].controlledObjects.Add(DungeonManager.instance.hackableObjects[controlAssignment.y]);
            }
        }
    }

    internal DungeonTile GetTile(int x, int z)
    {
        return tileGrid[x][z];
    }

    internal void ExploreStartingArea(Vector3Int startPoint)
    {
        foreach(DungeonTile[] row in tileGrid)
        {
            foreach (DungeonTile tile in row)
            {
                if (TileDistance(tile, tileGrid[startPoint.x][startPoint.z]) < STARTING_EXPLORE_DISTANCE)
                {
                    tile.Reveal();
                }
            }
        }
    }

    internal void GeneratePorts(Vector3Int[] portLocations)
    {
        foreach (Vector3Int portLocation in portLocations)
        {
            DungeonTile tile = tileGrid[portLocation.x][portLocation.z];
            Port newPort = Instantiate(port, tile.GetOccupyingCoordinates(false,true), Quaternion.identity).GetComponent<Port>();
            DungeonManager.instance.hackableObjects.Add(newPort);
            newPort.myTile = tile;
        }
    }

    internal void CreateDeploymentZone(Vector3Int deploymentCoords)
    {
        Instantiate(deploymentZone);
        DeploymentZone zone = FindObjectOfType<DeploymentZone>();
        zone.myCoords = deploymentCoords;
        zone.transform.position = tileGrid[deploymentCoords.x][deploymentCoords.z].GetOccupyingCoordinates(true,true);

    }

    internal List<EnemyProgram> GenerateEnemies(Vector3Int[] enemySpawns)
    {
        List<EnemyProgram> enemies = new List<EnemyProgram>();
        foreach(Vector3Int spawn in enemySpawns)
        {
            EnemyProgram newProgram = Instantiate(enemyPrefabs[spawn.y]).GetComponent<EnemyProgram>();
            enemies.Add(newProgram);
            DungeonManager.instance.DeploySecurity(newProgram, tileGrid[spawn.x][spawn.z]);
        }
        return enemies;
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
        int minX = Math.Max(0, program.myTile.xCoord - program.GetSight());
        int minZ = Math.Max(0, program.myTile.zCoord - program.GetSight());
        int maxX = Math.Min(tileGrid.Length-1, program.myTile.xCoord + program.GetSight());
        int maxZ = Math.Min(tileGrid[0].Length-1, program.myTile.zCoord + program.GetSight());
        for(int i=minX;i<=maxX; i++)
        {
            for(int j=minZ; j<=maxZ;j++)
            {
                if (TileDistance(program.myTile, tileGrid[i][j]) <= program.GetSight() && IsInLineOfSight(program, tileGrid[i][j]))
                {
                    seenTiles.Add(tileGrid[i][j]);
                }
            }
        }
        return seenTiles;
    }

    internal bool CanDeployHere(DungeonTile dungeonTile)
    {
        return (!dungeonTile.isOccupied)&&((dungeonTile.GetHeight() >= 0&&!dungeonTile.isBlocked)|| Program.selectedProgram.IsFlying());
    }

    internal void FogOfWarRender(Program[] playerPrograms)
    {
        foreach(DungeonTile[] row in tileGrid)
        {
            foreach(DungeonTile tile in row)
            {
                if(IsSeenByPlayer(tile,playerPrograms)||IsSeenByTerminal(tile))
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

    private bool IsSeenByTerminal(DungeonTile tile)
    {
        foreach(Terminal terminal in DungeonManager.instance.terminals)
        {
            if(terminal.IsHacked()&&terminal.maxCamX>=0)
            {
                if (tile.xCoord >= terminal.minCamX && tile.xCoord <= terminal.maxCamX && tile.zCoord >= terminal.minCamZ && tile.zCoord <= terminal.maxCamZ)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsSeenByPlayer(DungeonTile tile, Program[] playerPrograms)
    {
        foreach(Program program in playerPrograms)
        {
            if(TileDistance(tile, program.myTile) <= program.GetSight()&&IsInLineOfSight(program,tile))
            {
                return true;
            }
        }
        return false;
    }

    internal int TileDistance(DungeonTile tile, DungeonTile myTile)
    {
        return Math.Abs(tile.xCoord - myTile.xCoord) + Math.Abs(tile.zCoord - myTile.zCoord);
    }

    internal bool IsInLineOfSight(Program observer, DungeonTile tile)
    {
        Vector3 direction = (tile.GetOccupyingCoordinates(true,false) - observer.myTile.GetOccupyingCoordinates(observer.IsFlying(),false)).normalized;
        float distance =(tile.GetOccupyingCoordinates(true,false) -observer.myTile.GetOccupyingCoordinates(observer.IsFlying(),false)).magnitude;
        return !Physics.Raycast(observer.myTile.GetOccupyingCoordinates(observer.IsFlying(),false), direction, distance, LayerMask.GetMask("Ground"));
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
