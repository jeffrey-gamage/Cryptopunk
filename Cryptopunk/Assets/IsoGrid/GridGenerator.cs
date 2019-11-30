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
    private int gridSize;
    internal class Room
    {
        int size;
        internal List<Vector3Int> tiles;
        internal Room[] connections;
        internal List<Vector3Int> leftEdgeTiles;
        internal List<Vector3Int> rightEdgeTiles;
        internal List<Vector3Int> foreEdgeTiles;
        internal List<Vector3Int> aftEdgeTiles;

        internal Room(int size)
        {
            this.size = size;
            connections = new Room[Math.Max(2, size / 3)];
        }
        private Vector3Int GetRandomDirection()
        {
            int direction = Random.Range(0, 3);
            if(direction==0)
            {
                return Vector3Int.left;
            }
            if(direction==1)
            {
                return new Vector3Int(0, 0, 1);
            }
            if(direction==2)
            {
                return Vector3Int.right;
            }
            return new Vector3Int(0, 0, -1);
        }
        
        internal void GenerateTiles()
        {
            int minX;
            int minZ;
            int maxX;
            int maxZ;

            int roomLength = Random.Range(size, size * 2);
            int roomWidth = size * 3 - roomLength;
            tiles = new List<Vector3Int>();
            if (connections[0] == null)
            {
                minX = 0;
                minZ = 0;
                maxX = roomLength;
                maxZ = roomWidth;
            }
            else
            {
                Vector3Int direction = GetRandomDirection();
                Vector3Int connector;
                do
                {
                    connector = connections[0].TakeRandomEdge(direction);
                }
                while (connector == Vector3Int.up);
                tiles.Add(new Vector3Int(connector.x + direction.x, 0, connector.z + direction.z));
                if (direction.z > 0)
                {
                    minZ = connector.z + 1;
                    maxZ = minZ + roomWidth;
                    minX = Random.Range(connector.x - roomLength, connector.x);
                    maxX = minX + roomLength;
                }
                else if (direction.x < 0)
                {
                    maxX = connector.x - 1;
                    minX = maxX - roomLength;
                    minZ = Random.Range(connector.z - roomWidth, connector.z);
                    maxZ = minZ + roomWidth;
                }
                else if (direction.z < 0)
                {
                    maxZ = connector.z - 1;
                    minZ = maxZ - roomWidth;
                    minX = Random.Range(connector.x - roomLength, connector.x);
                    maxX = minX + roomLength;
                }
                else
                {
                    minX = connector.x + 1;
                    maxX = minX + roomLength;
                    minZ = Random.Range(connector.z - roomWidth, connector.z);
                    maxZ = minZ + roomWidth;
                }
            }
            for (int i=minX;i<maxX;i++)
            {
                for(int j=minZ;j<maxZ;j++)
                {
                    tiles.Add(new Vector3Int(i,0,j));
                }
            }
            foreEdgeTiles = new List<Vector3Int>();
            leftEdgeTiles = new List<Vector3Int>();
            aftEdgeTiles = new List<Vector3Int>();
            rightEdgeTiles = new List<Vector3Int>();
            foreach (Vector3Int tile in tiles)
            {
                if (tile.x == minX)
                {
                    leftEdgeTiles.Add(tile);
                }
                else if (tile.x == maxX - 1)
                {
                    rightEdgeTiles.Add(tile);
                }
                else if (tile.z == minZ )
                {
                    aftEdgeTiles.Add(tile);
                }
                else if(tile.z == maxZ - 1)
                {
                    foreEdgeTiles.Add(tile);
                }
            }
        }

        private Vector3Int TakeRandomEdge(Vector3Int direction)
        {
            Vector3Int randomEdge;
            if (direction.z>0&&foreEdgeTiles.Count>0)
            {
                randomEdge = foreEdgeTiles[Random.Range(0, foreEdgeTiles.Count)%foreEdgeTiles.Count];
                foreEdgeTiles.Remove(randomEdge);
            }
            else if (direction.x<0&&leftEdgeTiles.Count>0)
            {
                randomEdge = leftEdgeTiles[Random.Range(0, leftEdgeTiles.Count)%leftEdgeTiles.Count];
                leftEdgeTiles.Remove(randomEdge);
            }
            else if (direction.z<0&&aftEdgeTiles.Count>0)
            {
                randomEdge = aftEdgeTiles[Random.Range(0, aftEdgeTiles.Count)%aftEdgeTiles.Count];
                aftEdgeTiles.Remove(randomEdge);
            }
            else if(direction.x>0&&rightEdgeTiles.Count>0)
            {
                randomEdge = rightEdgeTiles[Random.Range(0, rightEdgeTiles.Count)%rightEdgeTiles.Count];
                rightEdgeTiles.Remove(randomEdge);
            }
            else
            {
                return Vector3Int.up;//signal to process that we couldn't find an edge "something is up"
            }
            return randomEdge;
        }
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
        int minX = 0;
        int minZ = 0;
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
                if(tileCoords.x>gridSize)
                {
                    gridSize = tileCoords.x;
                }
                if (tileCoords.z > gridSize)
                {
                    gridSize = tileCoords.z;
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
        gridSize += Math.Min(minX, minZ) * -1;
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
        int[][] rows = new int[gridSize][];
        for (int i = 0; i < gridSize; i++)
        {
            rows[i] = new int[gridSize];
            for (int j = 0; j < gridSize; j++)
            {
                rows[i][j] = -1;
            }
        }
        foreach (Room room in rooms)
        {
            foreach (Vector3Int tileCoords in room.tiles)
            {
                rows[tileCoords.x][tileCoords.z] = tileCoords.y;
            }
        }
        return rows;
    }
}
