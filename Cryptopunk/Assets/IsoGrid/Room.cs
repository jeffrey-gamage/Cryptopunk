using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room
{
    int size;
    static readonly Vector3Int failToFind = new Vector3Int(0, -3, 0);
    internal List<Vector3Int> tiles;
    internal Room[] connections;
    internal List<Vector3Int> leftEdgeTiles;
    internal List<Vector3Int> rightEdgeTiles;
    internal List<Vector3Int> foreEdgeTiles;
    internal List<Vector3Int> aftEdgeTiles;
    internal List<RampCoordinates> rampCoordinates;

    internal Room(int size)
    {
        this.size = size;
        connections = new Room[Mathf.Max(2, Mathf.Min(size / 3, 4))];
        rampCoordinates = new List<RampCoordinates>();
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
        int minX=0;
        int minZ=0;
        int maxX=0;
        int maxZ=0;
        int roomY =0;

        int roomLength = Random.Range(size, size * 2);
        int roomWidth = size * 3 - roomLength;
        tiles = new List<Vector3Int>();
        if (connections[0] != null)
        {
            Vector3Int direction = GetRandomDirection();
            Vector3Int connector;
            int attempts = 5;
            do
            {
                connector = connections[0].TakeRandomEdge(direction);
                attempts--;
            }
            while (connector == failToFind && attempts > 0);
            if (connector != failToFind)
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
            foreEdgeTiles.Remove(randomEdge);
        }
        else if (direction.x < 0 && leftEdgeTiles.Count > 0)
        {
            randomEdge = leftEdgeTiles[Random.Range(0, leftEdgeTiles.Count) % leftEdgeTiles.Count];
            leftEdgeTiles.Remove(randomEdge);
        }
        else if (direction.z < 0 && aftEdgeTiles.Count > 0)
        {
            randomEdge = aftEdgeTiles[Random.Range(0, aftEdgeTiles.Count) % aftEdgeTiles.Count];
            aftEdgeTiles.Remove(randomEdge);
        }
        else if (direction.x > 0 && rightEdgeTiles.Count > 0)
        {
            randomEdge = rightEdgeTiles[Random.Range(0, rightEdgeTiles.Count) % rightEdgeTiles.Count];
            rightEdgeTiles.Remove(randomEdge);
        }
        else
        {
            return failToFind;//signal to process that we couldn't find an edge - because vectors are not nullable
        }
        return randomEdge;
    }
}
