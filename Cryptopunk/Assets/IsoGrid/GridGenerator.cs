using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator 
{
    [SerializeField] int minNumRooms = 3;
    [SerializeField] int minRoomSize = 3;
    private int mapSize;
    private int connectivity;
    private int verticality;
    private List<Room> rooms;
    private int gridX;
    private int gridZ;
   

    public GridGenerator(int mapSize,int connectivity, int verticality)
    {
        this.mapSize = mapSize;
        this.connectivity = connectivity;
        this.verticality = verticality;

        rooms = new List<Room>();
        int roomsGeneratedSize = 0;
        while(roomsGeneratedSize<mapSize)
        {
            int newRoomSize = Random.Range(minRoomSize, Math.Min(mapSize/minNumRooms,mapSize - roomsGeneratedSize + 1));
            rooms.Add(new Room(newRoomSize));
            roomsGeneratedSize += newRoomSize;
        }
        rooms[0].GenerateTiles();
        for(int i=1;i<rooms.Count;i++)
        {
            MakeFirstConnection(i, rooms[i]);
            rooms[i].GenerateTiles();
        }
        DefineGridBoundaries();
    }

    private void DefineGridBoundaries()
    {
        int minX = 999;
        int minZ = 999;
        gridX = 0;
        gridZ = 0;
        foreach(Room room in rooms)
        {
            foreach(Vector3Int tileCoords in room.tiles)
            {
                if(tileCoords.x<minX)
                {
                    minX =tileCoords.x;
                }
                if(tileCoords.z<minZ)
                {
                    minZ = tileCoords.z;
                }
                if(tileCoords.x>gridX)
                {
                    gridX = tileCoords.x;
                }
                if (tileCoords.z > gridX)
                {
                    gridZ = tileCoords.z;
                }
            }
        }
        for(int i=0;i<rooms.Count;i++)
        {
            for(int j=0;j<rooms[i].tiles.Count;j++)
            {
                rooms[i].tiles[j] += new Vector3Int(-1, 0, 0) * minX+new Vector3Int(0,0,-1)*minZ;
            }
        }
        gridX += minX * -1+1;
        gridZ += minZ * -1 + 1;
    }

    private void MakeFirstConnection(int i, Room room)
    {
        while(room.connections[0]==null)
        {
            Room toConnect = rooms[Random.Range(0, i)];
            for(int j=0;j<toConnect.connections.Length;j++)
            {
                if(toConnect.connections[j]==null)
                {
                    toConnect.connections[j] = room;
                    room.connections[0] = toConnect;
                    break;
                }
            }
        }
    }

    internal int[][] GetGrid()
    {
        int[][] rows = new int[gridX][];
        for (int i = 0; i < gridX; i++)
        {
            rows[i] = new int[gridX];
            for (int j = 0; j < gridZ; j++)
            {
                rows[i][j] = -1;
            }
        }
        foreach (Room room in rooms)
        {
            foreach (Vector3Int tileCoords in room.tiles)
            {
                if (tileCoords.x >= gridX || tileCoords.z >= gridX)
                {
                    Debug.LogWarning("tile coords out of bounds - tile coords: " + tileCoords.x.ToString() + ", " + tileCoords.z.ToString() + ", grid max: " + gridX.ToString());
                }
                else
                {
                    rows[tileCoords.x][tileCoords.z] = tileCoords.y;
                }
            }
        }
        return rows;
    }
}
