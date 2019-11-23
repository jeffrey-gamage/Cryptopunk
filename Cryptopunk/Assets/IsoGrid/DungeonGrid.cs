﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField] GameObject gridTile;
    [SerializeField] GameObject ramp;
    [SerializeField] GameObject[] enemyPrefabs;
    private DungeonTile[][] tileGrid;
    [SerializeField] int numSegments = 3;
    [SerializeField] int searchSize = 8;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    internal void GenerateEnemies()
    {
        EnemyProgram newProgram = Instantiate(enemyPrefabs[0]).GetComponent<EnemyProgram>();
        DungeonManager.instance.DeploySecurity(newProgram, tileGrid[1][1]);
        newProgram.waypoints = new List<DungeonTile>();
        newProgram.waypoints.Add(tileGrid[1][1]);
        newProgram.waypoints.Add(tileGrid[1][6]);
        newProgram.waypoints.Add(tileGrid[6][6]);
        newProgram.waypoints.Add(tileGrid[6][1]);
    }

    internal void GenerateRamps()
    {
        Ramp newRamp = Instantiate(ramp).GetComponent<Ramp>();
        newRamp.SetDirection(Ramp.Direction.Back);
        tileGrid[4][1].SetRamp(newRamp);

        newRamp = Instantiate(ramp).GetComponent<Ramp>();
        newRamp.SetDirection(Ramp.Direction.Left);
        tileGrid[1][4].SetRamp(newRamp);

        newRamp = Instantiate(ramp).GetComponent<Ramp>();
        newRamp.SetDirection(Ramp.Direction.Forward);
        tileGrid[3][6].SetRamp(newRamp);

        newRamp = Instantiate(ramp).GetComponent<Ramp>();
        newRamp.SetDirection(Ramp.Direction.Right);
        tileGrid[6][3].SetRamp(newRamp);
    }

    internal DungeonTile GetNewSearchLocation(DungeonTile searcherTile)
    {
        DungeonTile newSearchLocation;
        do
        {
            newSearchLocation = tileGrid[Random.Range(Math.Max(0,searcherTile.xCoord-searchSize), Math.Min(tileGrid.Length,searcherTile.xCoord+searchSize))]
                [Random.Range(Math.Max(0, searcherTile.zCoord - searchSize), Math.Min(tileGrid.Length, searcherTile.zCoord + searchSize))];
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
            isSeen = isSeen || TileDistance(tile, program.myTile) <= program.sight;
        }
        return isSeen;
    }

    internal int TileDistance(DungeonTile tile, DungeonTile myTile)
    {
        return Math.Abs(tile.xCoord - myTile.xCoord) + Math.Abs(tile.zCoord - myTile.zCoord);
    }

    internal List<DungeonTile> FindPath(DungeonTile start, DungeonTile end, int pathLength, bool isFlying)
    {
        int[][] distances = new int[tileGrid.Length][];
        for(int i=0;i<distances.Length;i++)
        {
            distances[i] = new int[tileGrid.Length];
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
                if(IsValidCoordinates(path[path.Count-1].xCoord+1,path[path.Count-1].zCoord)&&
                    distances[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]==currentDistance-1&&
                    IsPassable(path[path.Count-1],tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]);
                }
                else if (IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord+1) &&
                    distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord+1] == currentDistance - 1&&
                    IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]))
                {
                     path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]);
                }
                else if (IsValidCoordinates(path[path.Count - 1].xCoord - 1, path[path.Count - 1].zCoord) &&
                    distances[path[path.Count - 1].xCoord-1][path[path.Count - 1].zCoord] == currentDistance - 1&&
                    IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]))
                {
                     path.Add(tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]);
                }
               else if (IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord - 1) &&
                    distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1] == currentDistance - 1&&
                    IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]))
                {
                    path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]);
                }
                else
                {
                    Debug.Log("Rejected all path options");
                    foreach(DungeonTile tile in path)
                    {
                        Debug.Log(tile.xCoord.ToString() + ", " + tile.zCoord.ToString());
                    }
                    break;
                }
            }
        }

        return path;
    }
    private void SetDistanceRecursive(ref int[][] distances, int currentDistance,DungeonTile tile,int maxDistance,bool isFlying)
    {
        if (distances[tile.xCoord][tile.zCoord] > currentDistance)
        {
            distances[tile.xCoord][tile.zCoord] = currentDistance;
            if (currentDistance < maxDistance)
            {
                if (IsValidCoordinates(tile.xCoord + 1, tile.zCoord) && IsPassable(tile, tileGrid[tile.xCoord + 1][tile.zCoord]))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord + 1][tile.zCoord], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord - 1, tile.zCoord) && IsPassable(tile, tileGrid[tile.xCoord - 1][tile.zCoord]))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord - 1][tile.zCoord], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord, tile.zCoord+1) && IsPassable(tile, tileGrid[tile.xCoord][tile.zCoord+1]))
                {
                    SetDistanceRecursive(ref distances, currentDistance + 1, tileGrid[tile.xCoord][tile.zCoord+1], maxDistance, isFlying);
                }
                if (IsValidCoordinates(tile.xCoord, tile.zCoord-1) && IsPassable(tile, tileGrid[tile.xCoord][tile.zCoord-1]))
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
        if(zCoord<0||zCoord>=tileGrid.Length)
        {
            return false;
        }
        return true;
    }
}
