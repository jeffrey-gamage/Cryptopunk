using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room
{
    public static readonly float PROBABILITY_OF_LEDGE_IN_ROOM = 0.5f;
    int size;
    static readonly Vector3Int failToFind = new Vector3Int(0, -3, 0);
    internal List<Vector3Int> tiles;
    internal Room[] connections;
    internal List<Vector3Int> leftEdgeTiles;
    internal List<Vector3Int> rightEdgeTiles;
    internal List<Vector3Int> foreEdgeTiles;
    internal List<Vector3Int> aftEdgeTiles;
    internal List<RampCoordinates> rampCoordinates;

    private int roomLength;
    private int roomWidth;
    private int minX;
    private int minZ;
    private int maxX;
    private int maxZ;
    private int roomY;

    internal Room(int size)
    {
        this.size = size;
        connections = new Room[Mathf.Max(2, Mathf.Min(size / 3, 4))];
        rampCoordinates = new List<RampCoordinates>();
    }

    internal Room(int minX, int maxX, int minZ, int maxZ, int height, List<RampCoordinates> rampCoordinates)
        //directly create rooms for the tutorial. not compatible with procedural generation
    {
        tiles = new List<Vector3Int>();
        this.rampCoordinates = rampCoordinates;
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minZ; j < maxZ; j++)
            {
                tiles.Add(new Vector3Int(i, height, j));
            }
        }
    }
    private Vector3Int GetRandomDirection()
    {
        int direction = Random.Range(0, 3);
        if (direction == 0)
        {
            return Vector3Int.left;
        }
        if (direction == 1)
        {
            return new Vector3Int(0, 0, 1);
        }
        if (direction == 2)
        {
            return Vector3Int.right;
        }
        return new Vector3Int(0, 0, -1);
    }

    internal void GenerateTiles(int vert)
    {
        minX=0;
        minZ=0;
        maxX=0;
        maxZ=0;
        roomY =0;

        roomLength = Random.Range(size, size * 2);
        roomWidth = size * 3 - roomLength;
        tiles = new List<Vector3Int>();
        if (connections[0] != null)
        {
            Vector3Int direction;
            Vector3Int connector;
            int attempts = 5;
            do
            {
                direction = GetRandomDirection();
                connector = connections[0].TakeRandomEdge(direction);
                attempts--;
            }
            while (connector == failToFind && attempts > 0);
            if (connector != failToFind)
            {
                roomY = RandomizeHeight(connector.y, vert);
                Vector3Int connection = new Vector3Int(connector.x + direction.x, roomY, connector.z + direction.z);
                tiles.Add(connection);
                if (connection.y != connector.y)
                {
                    RampCoordinates newCoordinates;
                    newCoordinates.coord1 = connection;
                    newCoordinates.coord2 = connector;
                    rampCoordinates.Add(newCoordinates);
                }
                if (direction.z > 0)
                {
                    minZ = connector.z + 1;
                    maxZ = minZ + roomWidth;
                    minX = Random.Range(connector.x + 1 - roomLength, connector.x);
                    maxX = minX + roomLength;
                }
                else if (direction.x < 0)
                {
                    maxX = connector.x - 1;
                    minX = maxX - roomLength;
                    minZ = Random.Range(connector.z + 1 - roomWidth, connector.z);
                    maxZ = minZ + roomWidth;
                }
                else if (direction.z < 0)
                {
                    maxZ = connector.z - 1;
                    minZ = maxZ - roomWidth;
                    minX = Random.Range(connector.x + 1 - roomLength, connector.x);
                    maxX = minX + roomLength;
                }
                else
                {
                    minX = connector.x + 1;
                    maxX = minX + roomLength;
                    minZ = Random.Range(connector.z + 1 - roomWidth, connector.z);
                    maxZ = minZ + roomWidth;
                }
            }
        }
        if (connections[0] == null)
        {
            minX = 0;
            minZ = 0;
            maxX = roomLength;
            maxZ = roomWidth;
            roomY = Random.Range(0, vert);
        }
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minZ; j < maxZ; j++)
            {
                tiles.Add(new Vector3Int(i, roomY, j));
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
            else if (tile.z == minZ)
            {
                aftEdgeTiles.Add(tile);
            }
            else if (tile.z == maxZ - 1)
            {
                foreEdgeTiles.Add(tile);
            }
        }
    }

    internal void DivideIntoChambers(ref List<Vector3Int> firewalls)
    {
        if (tiles.Count>36)
        {
            Debug.Log("room is large enought to divide");
            bool pivotIsXdimension = OrientPivot();
            int pivot = selectPivot(pivotIsXdimension);

            Debug.Log("pivot at " + pivot.ToString() + " x = " + pivotIsXdimension.ToString());
            if (Random.value > PROBABILITY_OF_LEDGE_IN_ROOM)
            {
                Debug.Log("dividing with firewalls");
                PlaceDividingFirewalls(pivotIsXdimension, pivot, ref firewalls);
            }
            else
            {
                Debug.Log("dividing with ramps");
                CreateLedgeAndRamps(pivotIsXdimension, pivot);
            }
        }
        Debug.Log("room is too small to divide");
    }

    private void CreateLedgeAndRamps(bool pivotIsXdimension, int pivot)
    {
        int newHeight = GetNewHeight(roomY);
        List<Vector3Int> rampCandidates = new List<Vector3Int>();
        if (pivotIsXdimension)
        {
            for (int i=0;i<tiles.Count;i++)
            {
                if (tiles[i].z > pivot&&connections[0].GetCentre().z<pivot)
                {
                    tiles[i]=new Vector3Int(tiles[i].x,newHeight,tiles[i].z);
                }
                else if (tiles[i].z < pivot && connections[0].GetCentre().z > pivot)
                {
                    tiles[i] = new Vector3Int(tiles[i].x, newHeight, tiles[i].z);
                }
                else if(tiles[i].z==pivot)
                {
                    rampCandidates.Add(tiles[i]);
                }
            }
            if (rampCandidates.Count > 0)
            {
                RampCoordinates rampCoords;
                rampCoords.coord1 = rampCandidates[Random.Range(0, rampCandidates.Count)];
                rampCoords.coord2 = rampCoords.coord1 + Vector3Int.right;
                rampCoordinates.Add(rampCoords);
            }
            else
            {
                Debug.LogWarning("no valid tiles for ramp!");
            }
        }
        else
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].x > pivot&&connections[0].GetCentre().x<pivot)
                {
                    tiles[i] = new Vector3Int(tiles[i].x, newHeight, tiles[i].z);
                }
                else if (tiles[i].x < pivot && connections[0].GetCentre().x > pivot)
                {
                    tiles[i] = new Vector3Int(tiles[i].x, newHeight, tiles[i].z);
                }
                else if (tiles[i].x == pivot)
                {
                    rampCandidates.Add(tiles[i]);
                }
            }
            if (rampCandidates.Count > 0)
            {
                RampCoordinates rampCoords;
                rampCoords.coord1 = rampCandidates[Random.Range(0, rampCandidates.Count)];
                rampCoords.coord2 = rampCoords.coord1 + new Vector3Int(0,0,1);
                rampCoordinates.Add(rampCoords);
            }
            else
            {
                Debug.LogWarning("no valid tiles for ramp!");
            }
        }
    }

    private int GetNewHeight(int roomY)
    {
        if (roomY==0)
        {
            return 1;
        }
        if(roomY ==3)
        {
            return 2;
        }
        if(Random.value>0.5f)
        {
            return roomY + 1;
        }
        return roomY - 1;
    }

    private void PlaceDividingFirewalls(bool pivotIsXdimension, int pivot, ref List<Vector3Int> firewalls)
    {
        if(pivotIsXdimension)
        {
            foreach(Vector3Int tile in tiles)
            {
                if (tile.z == pivot)
                {
                    Debug.Log("Adding firewall at  " + tile.ToString());
                    firewalls.Add(tile);
                }
            }
        }
        else
        {
            foreach (Vector3Int tile in tiles)
            {
                if (tile.x == pivot)
                {
                    firewalls.Add(tile);
                }
            }
        }
    }

    private int selectPivot(bool pivotIsXdimension)
    {
        if(pivotIsXdimension)
        {
            int zPivot = Random.Range(minZ + 3, maxZ - 3);
            Debug.Log("minZ = " + minZ.ToString() + ", maxZ = " + maxZ.ToString() + "pivot = " + zPivot.ToString());
            return zPivot;
        }
        int xPivot = Random.Range(minX + 3, maxX - 3);
        Debug.Log("minX = " + minX.ToString() + ", maxX = " + maxX.ToString() + "pivot = " + xPivot.ToString());
        return xPivot;
    }

    private bool OrientPivot()
    {
        if (roomLength < 7)
            return true;
        if (roomWidth < 7)
            return false;
        return Random.value > 0.5f;
    }

    internal Vector3Int GetCentre()
    {
        int avgX = 0;
        int avgZ = 0;
        foreach(Vector3Int coord in tiles)
        {
            avgX += coord.x;
            avgZ += coord.z;
        }
        avgX /= tiles.Count;
        avgZ /= tiles.Count;
        return new Vector3Int(avgX, 0, avgZ);
    }

    private int RandomizeHeight(int y, int vert)
    {
        return Mathf.Clamp(Random.Range(y - 1, y + 1), 0, vert);
    }

    private Vector3Int TakeRandomEdge(Vector3Int direction)
    {
        Vector3Int randomEdge;
        if (direction.z > 0 && foreEdgeTiles.Count > 0)
        {
            randomEdge = foreEdgeTiles[Random.Range(0, foreEdgeTiles.Count) % foreEdgeTiles.Count];
            foreEdgeTiles.Clear();
        }
        else if (direction.x < 0 && leftEdgeTiles.Count > 0)
        {
            randomEdge = leftEdgeTiles[Random.Range(0, leftEdgeTiles.Count) % leftEdgeTiles.Count];
            leftEdgeTiles.Clear();
        }
        else if (direction.z < 0 && aftEdgeTiles.Count > 0)
        {
            randomEdge = aftEdgeTiles[Random.Range(0, aftEdgeTiles.Count) % aftEdgeTiles.Count];
            aftEdgeTiles.Clear();
        }
        else if (direction.x > 0 && rightEdgeTiles.Count > 0)
        {
            randomEdge = rightEdgeTiles[Random.Range(0, rightEdgeTiles.Count) % rightEdgeTiles.Count];
            rightEdgeTiles.Clear();
        }
        else
        {
            return failToFind;//signal to process that we couldn't find an edge - because vectors are not nullable
        }
        return randomEdge;
    }
}
