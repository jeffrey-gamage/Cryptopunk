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
    internal static GridGenerator instance;

    private int mapSize;
    private int connectivity;
    private int verticality;
    internal List<Room> rooms;
    private int gridX;
    private int gridZ;
    public GridGenerator(int numRooms, int corpID)
    {
        instance = this;
        rooms = new List<Room>();

        int firstRoomIndex = RoomDirectory.instance.GetFirstRoomIndex();
        int[] genRoomIndices = RoomDirectory.instance.GetGenRoomIndices(numRooms);
        int finalRoomIndex = RoomDirectory.instance.GetFinalRoomIndex();

        rooms.Add(Room.LoadFirstRoom(firstRoomIndex));
        Room previousRoom;
        Vector3Int roomCoords;
        int roomOrientation;

        if (numRooms > 1)
        {
            for (int i = 1; i < numRooms - 1; i++)
            {
                previousRoom = SelectRandomRoom();
                roomCoords = previousRoom.GetRandomExit();
                roomOrientation = previousRoom.GetOrientation(roomCoords);
                rooms.Add(Room.LoadGeneralRoom(genRoomIndices[i - 1], roomCoords, roomOrientation));
            }
            previousRoom = SelectRandomRoom();
            roomCoords = previousRoom.GetRandomExit();
            roomOrientation = previousRoom.GetOrientation(roomCoords);
            rooms.Add(Room.LoadFinalRoom(finalRoomIndex, roomCoords, roomOrientation));
        }
        DefineGridBoundaries();
    }
    protected GridGenerator()
    {
        instance = this;
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

    internal Vector3Int[] GetHubs()
    {
        List<Vector3Int> securityHubs = new List<Vector3Int>();
        foreach(Room room in rooms)
        {
            securityHubs.AddRange(room.securityHubs);
        }
        return securityHubs.ToArray();
    }

    internal Vector3Int[] GetDefences()
    {
        List<Vector3Int> defences = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            defences.AddRange(room.defences);
        }
        return defences.ToArray();
    }

    internal Vector3Int[] GetLoot()
    {
        List<Vector3Int> loot = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            loot.AddRange(room.loot);
        }
        return loot.ToArray();
    }

    internal Vector3Int[] GetEnemies()
    {
        List<Vector3Int> enemies = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            enemies.AddRange(room.enemies);
        }
        return enemies.ToArray();
    }

    internal Vector3Int[] GetPorts()
    {
        List<Vector3Int> ports = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            ports.AddRange(room.ports);
        }
        return ports.ToArray();
    }
    internal Vector3Int[] GetTerminals()
    {
        List<Vector3Int> terminals = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            terminals.AddRange(room.terminals);
        }
        return terminals.ToArray();
    }

    internal Vector3Int[] GetFirewalls()
    {
        List<Vector3Int> firewalls = new List<Vector3Int>();
        foreach (Room room in rooms)
        {
            firewalls.AddRange(room.firewalls);
        }
        return firewalls.ToArray();
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

    private Room SelectRandomRoom()
    {
        return rooms[Random.Range(0, rooms.Count)];
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
        gridX+=1- minX;
        gridZ+=1- minZ;
        RepositionToMinimizedGrid(minX, minZ);
    }

    private void RepositionToMinimizedGrid(int minX, int minZ)
    {
        foreach(Room room in rooms)
        {
            room.AdjustCoordinatesToMinimizedGrid(minX,minZ);
        }
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
