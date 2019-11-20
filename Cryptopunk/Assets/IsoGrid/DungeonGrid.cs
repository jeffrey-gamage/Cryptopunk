﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField] GameObject chasm;
    [SerializeField] GameObject grid0;
    [SerializeField] GameObject grid1;
    [SerializeField] GameObject grid2;
    [SerializeField] GameObject grid3;
    private DungeonTile[][] tileGrid;
    [SerializeField] int numSegments = 3;
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
                tileGrid[i][j] = Instantiate(grid0, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                tileGrid[i][j].SetHeight(gridHeights[i][j]);
                tileGrid[i][j].xCoord = i;
                tileGrid[i][j].zCoord = j;
            }
        }
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

    private int TileDistance(DungeonTile tile, DungeonTile myTile)
    {
        return Math.Abs(tile.xCoord - myTile.xCoord) + Math.Abs(tile.zCoord - myTile.zCoord);
    }

    //internal List<DungeonTile> FindPath(DungeonTile start, DungeonTile end, int pathLength, bool isFlyingPath)
    //{
    //    List<DungeonTile> path = new List<DungeonTile>();
    //    path.Add(start);
    //    while(path.Count<=pathLength&&(path.Count==0||path[path.Count-1]!=end))
    //    {
    //        bool blockedX = false;
    //        bool blockedZ = false;
    //        TryPathX(end, isFlyingPath, path, ref blockedX, ref blockedZ);
    //        TryPathZ(end, isFlyingPath, path, ref blockedX, ref blockedZ);
    //        if (blockedX && blockedZ)
    //        {
    //            //TODO: pathfind around obstacles
    //        }
    //    }
    //    return path;
    //}

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
                int minDistance =63;
                DungeonTile nextTile=null;
                if(IsValidCoordinates(path[path.Count-1].xCoord+1,path[path.Count-1].zCoord))
                {
                    nextTile = tileGrid[path[path.Count - 1].xCoord+1][path[path.Count - 1].zCoord];
                    minDistance = distances[path[path.Count - 1].xCoord+1][path[path.Count - 1].zCoord];
                }
                if (IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord+1))
                {
                    if (distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1] < minDistance)
                    {
                        nextTile = tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1];
                        minDistance = distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1];
                    }
                }
                if (IsValidCoordinates(path[path.Count - 1].xCoord - 1, path[path.Count - 1].zCoord))
                {
                    if (distances[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]< minDistance)
                    {
                        nextTile = tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord];
                        minDistance = distances[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord];
                    }
                }
                if (IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord - 1))
                {
                    if (distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1] < minDistance)
                    {
                        nextTile = tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1];
                        minDistance = distances[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1];
                    }
                }
                path.Add(nextTile);
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

    private void TryPathZ(DungeonTile end, bool isFlyingPath, List<DungeonTile> path, ref bool blockedX, ref bool blockedY)
    {
        if (end.zCoord > path[path.Count - 1].zCoord && IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord + 1))
        {
            if (!isFlyingPath && !IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord+1]))
            {
                blockedX = true;
            }
            else
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]);
            }
        }
        else if (end.zCoord < path[path.Count - 1].zCoord && IsValidCoordinates(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord - 1))
        {
            if (!isFlyingPath && !IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]))
            {
                blockedY = true;
            }
            else
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]);
            }
        }
    }

    private void TryPathX(DungeonTile end, bool isFlyingPath, List<DungeonTile> path, ref bool blockedX, ref bool blockedY)
    {
        if (end.xCoord > path[path.Count - 1].xCoord && IsValidCoordinates(path[path.Count - 1].xCoord + 1, path[path.Count - 1].zCoord))
        {
            if (!isFlyingPath && !IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]))
            {
                blockedX = true;
            }
            else
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]);
            }
        }
        else if (end.xCoord < path[path.Count - 1].xCoord && IsValidCoordinates(path[path.Count - 1].xCoord - 1, path[path.Count - 1].zCoord))
        {
            if (!isFlyingPath && !IsPassable(path[path.Count - 1], tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]))
            {
                blockedY = true;
            }
            else
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]);
            }
        }
    }

    private bool IsPassable(DungeonTile dungeonTile1, DungeonTile dungeonTile2)
        //determines if a transition is possible from tile1 to tile2
    {
        return (!dungeonTile2.isBlocked) && dungeonTile2.GetHeight() == dungeonTile1.GetHeight();
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
