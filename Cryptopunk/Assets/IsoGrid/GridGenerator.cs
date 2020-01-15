using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct RampCoordinates
{
    public Vector3Int coord1;
    public Vector3Int coord2;
}

public class GridGenerator 
{
    [SerializeField] int minNumRooms = 3;
    [SerializeField] int minRoomSize = 2;
    private int mapSize;
    private int connectivity;
    private int verticality;
    private List<Room> rooms;
    private int gridX;
    private int gridZ;
   
    protected GridGenerator()
    {
        //only used by tutorial grid generator;
        rooms = new List<Room>();
        rooms.Add(new Room(6, 15, 2, 8, 0, new List<RampCoordinates>()));
        rooms.Add(new Room(5, 7, 8, 11, 0, new List<RampCoordinates>()));
        rooms.Add(new Room(1, 5, 10, 12, 0, new List<RampCoordinates>()));
        RampCoordinates rampCoords1;
        rampCoords1.coord1 = new Vector3Int(8, 0, 17);
        rampCoords1.coord2 = new Vector3Int(7, 1, 17);
        List<RampCoordinates> rampCoordinates = new List<RampCoordinates>();
        rampCoordinates.Add(rampCoords1);
        rooms.Add(new Room(5, 10, 10, 18, 0,rampCoordinates));
        rooms.Add(new Room(5, 8, 16, 20, 1, new List<RampCoordinates>()));
        rooms.Add(new Room(10, 16, 12, 14, 0, new List<RampCoordinates>()));
        rooms.Add(new Room(13, 15, 9, 12, 0, new List<RampCoordinates>()));
        RampCoordinates rampCoords2;
        rampCoords2.coord1 = new Vector3Int(18, 0, 15);
        rampCoords2.coord2 = new Vector3Int(18, 1, 14);
        List<RampCoordinates> rampCoordinates2 = new List<RampCoordinates>();
        rampCoordinates2.Add(rampCoords2);
        rooms.Add(new Room(16, 21, 10, 19, 0, rampCoordinates2));
        RampCoordinates rampCoords3;
        rampCoords3.coord1 = new Vector3Int(19, 1, 10);
        rampCoords3.coord2 = new Vector3Int(20, 2, 10);
        List<RampCoordinates> rampCoordinates3 = new List<RampCoordinates>();
        rampCoordinates3.Add(rampCoords3);
        rooms.Add(new Room(18, 20, 10, 15, 1, rampCoordinates3));
        rooms.Add(new Room(20, 21, 10, 15, 2, new List<RampCoordinates>()));
        DefineGridBoundaries();
    }

    internal Vector3Int[] GetEnemies()
    {
        throw new NotImplementedException();
    }

    internal Vector3Int[] GetPorts()
    {
        //TODO
        throw new NotImplementedException();
    }

    internal Vector3Int GetDeploymentArea()
    {
        return rooms[0].GetCentre();
    }

    internal Vector2Int[] GetTerminalControlledObjects()
    {
        //TODO
        throw new NotImplementedException();
    }

    internal List<Vector3Int> GetStartingArea()
    {
        return rooms[0].tiles;
    }

    internal Vector3Int[] GetTerminals()
    {
        //TODO
        throw new NotImplementedException();
    }

    internal Vector3Int[] GetFirewalls()
    {
        //TODO
        return new Vector3Int[0];
    }

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
        rooms[0].GenerateTiles(verticality);
        for(int i=1;i<rooms.Count;i++)
        {
            MakeFirstConnection(i, rooms[i]);
            rooms[i].GenerateTiles(verticality);
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
                if (tileCoords.z > gridZ)
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
            for(int k=0;k<rooms[i].rampCoordinates.Count;k++)
            {
                RampCoordinates updatedCoords;
                updatedCoords.coord1 = rooms[i].rampCoordinates[k].coord1 + new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
                updatedCoords.coord2 = rooms[i].rampCoordinates[k].coord2 + new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
                rooms[i].rampCoordinates[k] = updatedCoords;
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
            rows[i] = new int[gridZ];
            for (int j = 0; j < gridZ; j++)
            {
                rows[i][j] = -1;
            }
        }
        foreach (Room room in rooms)
        {
            foreach (Vector3Int tileCoords in room.tiles)
            {
                if (tileCoords.x >= gridX)
                {
                    Debug.LogWarning("tile x out of bounds - tile coords: " + tileCoords.x.ToString() + ", " + tileCoords.z.ToString() + ", grid max: " + gridX.ToString());
                }
                else if(tileCoords.z >= gridZ)
                {
                    Debug.LogWarning("tile z out of bounds - tile coords: " + tileCoords.x.ToString() + ", " + tileCoords.z.ToString() + ", grid max: " + gridZ.ToString());
                }
                else
                {
                    rows[tileCoords.x][tileCoords.z] = Mathf.Max(rows[tileCoords.x][tileCoords.z],tileCoords.y);
                }
            }
        }
        return rows;
    }

    internal List<RampCoordinates> GetRamps()
    {
        List<RampCoordinates> allRampCoordinates= new List<RampCoordinates>();
        foreach( Room room in rooms)
        {
            foreach( RampCoordinates coordinates in room.rampCoordinates)
            {
                allRampCoordinates.Add(coordinates);
            }
        }
        return allRampCoordinates;
    }

}
