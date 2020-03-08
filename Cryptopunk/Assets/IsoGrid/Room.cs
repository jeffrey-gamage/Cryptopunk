﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room
{
    private static int baseDifficulty = 4;
    int size;
    static readonly Vector3Int failToFind = new Vector3Int(0, -3, 0);
    internal List<Vector3Int> tiles;
    internal Room[] connections;
    internal List<RampCoordinates> rampCoordinates;

    internal Vector3Int deploymentPoint = Vector3Int.down;
    internal Vector3Int entrance;
    internal List<Vector3Int> exits;

    internal List<Vector3Int> firewalls;
    internal List<Vector3Int> securityHubs;
    internal List<Vector3Int> defences;
    internal List<Vector3Int> loot;
    internal List<Vector3Int> ports;
    internal List<Vector3Int> terminals;
    internal List<Vector3Int> enemies;
    internal List<List<Vector3Int>> patrolRoutes;
    internal List<TerminalAssignment> terminalAssignments;
    internal bool isControlledByInternalTerminal = false;
    internal bool enemiesArePredefined = false;
    internal Vector3Int missionObj=Vector3Int.down;

    private int roomLength;
    private int roomWidth;
    private int roomMinX;
    private int roomMinZ;
    private int roomMaxX;
    private int roomMaxZ;
    private int roomY;

    internal enum Orientation
    {
        right,forward,left,back
    }

    internal Room(int size)
    {
        this.size = size;
        connections = new Room[Mathf.Max(2, Mathf.Min(size / 3, 4))];
        rampCoordinates = new List<RampCoordinates>();
    }

    private Room()
    {
        exits = new List<Vector3Int>();
        rampCoordinates = new List<RampCoordinates>();
        tiles = new List<Vector3Int>();
        firewalls = new List<Vector3Int>();
        securityHubs = new List<Vector3Int>();
        enemies = new List<Vector3Int>();
        defences = new List<Vector3Int>();
        loot = new List<Vector3Int>();
        ports = new List<Vector3Int>();
        terminals = new List<Vector3Int>();
        patrolRoutes = new List<List<Vector3Int>>();
    }

    internal void ChooseEnemies()
    {
        int difficultyBudget = baseDifficulty + PersistentState.instance.progress;
        if (!enemiesArePredefined)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (difficultyBudget > 0)
                {
                    int enemyIndex = DungeonManager.instance.grid.SelectEnemy(difficultyBudget);
                    enemies[i] += Vector3Int.up * enemyIndex;
                    difficultyBudget -= DungeonManager.instance.grid.GetEnemyRating(enemyIndex);
                }
            }
        }
    }

    internal bool Overlaps(Room newRoom)
    {
        if((roomMaxX>newRoom.roomMinX&&roomMinX<newRoom.roomMaxX)||(newRoom.roomMaxX > roomMinX && newRoom.roomMinX <roomMaxX))
        {
            if ((roomMaxZ > newRoom.roomMinZ && roomMinZ < newRoom.roomMaxZ) || (newRoom.roomMaxZ > roomMinZ && newRoom.roomMinZ < roomMaxZ))
            {
                return true;
            }
        }
        return false;
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

    internal void AssignEnemyTypes(Vector3Int[] predefinedEnemyTypes)
    {
        if (enemies.Count > 0)
        {
            for(int i= 0;i< enemies.Count;i++)
            {
                foreach (Vector3Int predefinedEnemy in predefinedEnemyTypes)
                {
                    if (enemies[i].x == predefinedEnemy.x && enemies[i].z == predefinedEnemy.z)
                    {
                        enemies[i] = predefinedEnemy;
                    }
                }

            }
        }
    }

    internal Vector3Int[] GetEnemies()
    {
        return enemies.ToArray();
    }

    internal static Room LoadFirstRoom(int roomIndex, RoomDirectory directory)
    {
        TextAsset roomFile = directory.STARTING_ROOMS[roomIndex];
        Room newRoom = new Room();
        newRoom.ParseRoomFile(roomFile.text);
        newRoom.SetRoomBoundaries();
        newRoom.ConfigureExitHeights();
        return newRoom;
    }
    internal static Room LoadGeneralRoom(int roomIndex, Vector3Int previousRoomExit, Orientation roomOrientation,RoomDirectory directory)
    {
        TextAsset roomFile = directory.GENERAL_ROOMS[roomIndex];
        Room newRoom = new Room();
        newRoom.ParseRoomFile(roomFile.text);
        newRoom.AdjustOrientation(roomOrientation);
        newRoom.AttachToPrevious(previousRoomExit, roomOrientation);
        newRoom.SetRoomBoundaries();
        newRoom.ConfigureExitHeights();
        return newRoom;
    }

    internal static Room LoadFinalRoom(int roomIndex, Vector3Int previousRoomExit, Orientation roomOrientation, RoomDirectory directory)
    {
        TextAsset roomFile = directory.FINAL_ROOMS[roomIndex];
        Room newRoom = new Room();
        newRoom.ParseRoomFile(roomFile.text);
        newRoom.AdjustOrientation(roomOrientation);
        newRoom.AttachToPrevious(previousRoomExit, roomOrientation);
        newRoom.SetRoomBoundaries();
        return newRoom;
    }
    private void SetRoomBoundaries()
    {
        roomMinX = 9999;
        roomMinZ = 9999;
        roomMaxX = -9999;
        roomMaxZ = -9999;
        foreach (Vector3Int tile in tiles)
        {
            if (tile.x < roomMinX)
            {
                roomMinX = tile.x;
            }
            if (tile.z < roomMinZ)
            {
                roomMinZ = tile.z;
            }
            if (tile.x > roomMaxX)
            {
                roomMaxX = tile.x;
            }
            if (tile.z > roomMaxZ)
            {
                roomMaxZ = tile.z;
            }
        }
    }

    private void AttachToPrevious(Vector3Int previousRoomExit, Orientation roomOrientation)
    {
        Vector3Int connectionPoint = entrance;
        Vector3Int connectorDirectionVector = Vector3Int.zero;
        switch (roomOrientation)
        {
            case Orientation.right:
                {
                    connectorDirectionVector = new Vector3Int(-1, 0, 0);
                    break;
                }
            case Orientation.left:
                {
                    connectorDirectionVector = new Vector3Int(1, 0, 0);
                    break;
                }
            case Orientation.forward:
                {
                    connectorDirectionVector = new Vector3Int(0, 0, -1);
                    break;
                }
            case Orientation.back:
                {
                    connectorDirectionVector = new Vector3Int(0, 0, 1);
                    break;
                }
        }

        connectionPoint += connectorDirectionVector;
        tiles.Add(connectionPoint);
        while (connectionPoint.y != previousRoomExit.y)
        {
            connectionPoint += connectorDirectionVector;
            if (connectionPoint.y > previousRoomExit.y)
            {
                connectionPoint += Vector3Int.down;
                RampCoordinates newRamp;
                newRamp.coord1 = connectionPoint;
                newRamp.coord2 = connectionPoint - connectorDirectionVector-Vector3Int.down;
                rampCoordinates.Add(newRamp);
                tiles.Add(connectionPoint);
            }
            else if (connectionPoint.y < previousRoomExit.y)
            {
                connectionPoint += Vector3Int.up;
                RampCoordinates newRamp;
                newRamp.coord1 = connectionPoint;
                newRamp.coord2 = connectionPoint - connectorDirectionVector-Vector3Int.up;
                rampCoordinates.Add(newRamp);
                tiles.Add(connectionPoint);
            }
        }

        Vector3Int translationVector = previousRoomExit-connectorDirectionVector - connectionPoint;
        TranslateEverything(translationVector);
    }

    internal bool ContainsTile(Vector3Int tileCoords)
    {
        return tileCoords.x <= roomMaxX && tileCoords.x >= roomMinZ && tileCoords.z <= roomMaxZ && tileCoords.z >= roomMinZ;
    }

    internal void ConnectTerminal(ref List<Terminal> terminals)
    {
        foreach(Terminal terminal in terminals)
        {
            foreach (TerminalAssignment assignment in terminalAssignments)
            {
                if (terminal.myTile.xCoord ==assignment.terminalLocation.x&&terminal.myTile.zCoord==assignment.terminalLocation.z)
                {
                    foreach(Vector3Int controlLocation in assignment.controlLocations)
                    {
                        foreach(Hackable controllableObject in DungeonManager.instance.hackableObjects)
                        {
                            if(controlLocation.x==controllableObject.myTile.xCoord&&controlLocation.z==controllableObject.myTile.zCoord)
                            {
                                terminal.controlledObjects.Add(controllableObject);
                            }
                        }
                    }
                }
            }
        }
    }

    internal void TranslateEverything(Vector3Int translationVector)
    {
        TranslateAll(ref tiles, translationVector);
        TranslateAll(ref exits, translationVector);
        TranslateAll(ref rampCoordinates, translationVector);
        TranslateAll(ref firewalls, translationVector);
        TranslateAll(ref securityHubs, translationVector);
        TranslateAll(ref enemies, translationVector);
        TranslateAll(ref defences, translationVector);
        TranslateAll(ref loot, translationVector);
        TranslateAll(ref ports, translationVector);
        TranslateAll(ref terminals, translationVector);
        if (isControlledByInternalTerminal)
        {
            TranslateAll(ref terminalAssignments, translationVector);
        }
        TranslateAll(ref patrolRoutes, translationVector);
        entrance += translationVector;
        if(missionObj!=Vector3Int.down)
        {
            missionObj += translationVector;
        }
    }

    private void TranslateAll(ref List<TerminalAssignment> terminalAssignments, Vector3Int translationVector)
    {
            foreach (TerminalAssignment terminalAssignment in terminalAssignments)
            {
                terminalAssignment.Translate(translationVector);
            }
    }

    internal bool HasTerminalTargets()
    {
        return (!isControlledByInternalTerminal)&&(defences.Count > 0 || firewalls.Count > 0);
    }

    private void TranslateAll(ref List<List<Vector3Int>> patrolRoutes, Vector3Int translationVector)
    {
        if (patrolRoutes.Count > 0)
        {
            for (int i = 0; i < patrolRoutes.Count; i++)
            {
                if (patrolRoutes[i].Count > 0)
                {
                    for (int j = 0; j < patrolRoutes[i].Count; j++)
                    {
                        patrolRoutes[i][j] += translationVector;
                    }
                }
            }
        }
    }

    private void TranslateAll(ref List<RampCoordinates> rampCoordinates, Vector3Int translationVector)
    {
        if (rampCoordinates.Count > 0)
        {
            for (int i = 0; i < rampCoordinates.Count; i++)
            {
                RampCoordinates translatedCoords;
                translatedCoords.coord1 = rampCoordinates[i].coord1+ translationVector;
                translatedCoords.coord2 = rampCoordinates[i].coord2+translationVector;
                rampCoordinates[i] = translatedCoords;
            }
        }
    }

    private void TranslateAll(ref List<Vector3Int> coordinates, Vector3Int translationVector)
    {
        if (coordinates.Count > 0)
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i] += translationVector;
            }
        }
    }

    private void AdjustOrientation(Orientation roomOrientation)
    {
        if(roomOrientation==Orientation.left)
        {
            FlipX(ref tiles);
            FlipX(ref exits);
            FlipX(ref rampCoordinates);
            FlipX(ref firewalls);
            FlipX(ref securityHubs);
            FlipX(ref enemies);
            FlipX(ref defences);
            FlipX(ref loot);
            FlipX(ref ports);
            FlipX(ref terminals);
            FlipX(ref patrolRoutes);
            missionObj += new Vector3Int(1, 0, 0) * (roomMaxX -missionObj.x * 2);
            entrance += new Vector3Int(1, 0, 0) * (roomMaxX - entrance.x * 2);
        }
        if(roomOrientation==Orientation.forward)
        {
            FlipDiagonal(ref tiles);
            FlipDiagonal(ref exits);
            FlipDiagonal(ref rampCoordinates);
            FlipDiagonal(ref firewalls);
            FlipDiagonal(ref securityHubs);
            FlipDiagonal(ref enemies);
            FlipDiagonal(ref defences);
            FlipDiagonal(ref loot);
            FlipDiagonal(ref ports);
            FlipDiagonal(ref terminals);
            FlipDiagonal(ref patrolRoutes);
            missionObj = new Vector3Int(missionObj.z, missionObj.y, missionObj.x);
            entrance = new Vector3Int(entrance.z,entrance.y,entrance.x);
        }
        if(roomOrientation==Orientation.back||roomOrientation==Orientation.right)
        {
            FlipZ(ref tiles);
            FlipZ(ref exits);
            FlipZ(ref rampCoordinates);
            FlipZ(ref firewalls);
            FlipZ(ref securityHubs);
            FlipZ(ref enemies);
            FlipZ(ref defences);
            FlipZ(ref loot);
            FlipZ(ref ports);
            FlipZ(ref terminals);
            FlipZ(ref patrolRoutes);
            missionObj += new Vector3Int(0, 0, 1) * (roomMaxZ - missionObj.z * 2);
            entrance += new Vector3Int(0, 0, 1) * (roomMaxZ - entrance.z * 2);
        }
    }

    private void FlipZ(ref List<List<Vector3Int>> patrolRoutes)
    {
        if (patrolRoutes.Count > 0)
        {
            for(int i=0;i<patrolRoutes.Count;i++)
            {
                if (patrolRoutes[i].Count > 0)
                {
                    for (int j = 0; j < patrolRoutes[i].Count; j++)
                    {
                        patrolRoutes[i][j] += new Vector3Int(0, 0, 1) * (roomMaxZ - patrolRoutes[i][j].z * 2);
                    }
                }
            }
        }
    }

    private void FlipZ(ref List<RampCoordinates> rampCoordinates)
    {
        if (rampCoordinates.Count > 0)
        {
            for (int i = 0; i < rampCoordinates.Count; i++)
            {
                RampCoordinates flippedCoords;
                flippedCoords.coord1 = new Vector3Int(rampCoordinates[i].coord1.x, rampCoordinates[i].coord1.y, roomMaxZ -rampCoordinates[i].coord1.z);
                flippedCoords.coord2 = new Vector3Int(rampCoordinates[i].coord2.x, rampCoordinates[i].coord2.y, roomMaxZ - rampCoordinates[i].coord2.z);
                rampCoordinates[i] = flippedCoords;
            }
        }
    }

    private void FlipZ(ref List<Vector3Int> coordinates)
    {
        if (coordinates.Count > 0)
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i] += new Vector3Int(0,0,1) * (roomMaxZ - coordinates[i].z * 2);
            }
        }
    }

    private void FlipDiagonal(ref List<List<Vector3Int>> patrolRoutes)
    {
        if (patrolRoutes.Count > 0)
        {
            for (int i = 0; i < patrolRoutes.Count; i++)
            {
                if (patrolRoutes[i].Count > 0)
                {
                    for (int j = 0; j < patrolRoutes[i].Count; j++)
                    {
                        patrolRoutes[i][j] = new Vector3Int(patrolRoutes[i][j].z,patrolRoutes[i][j].y,patrolRoutes[i][j].x);
                    }
                }
            }
        }
    }

    private void FlipDiagonal(ref List<RampCoordinates> rampCoordinates)
    {
        if (rampCoordinates.Count > 0)
        {
            for (int i = 0; i < rampCoordinates.Count; i++)
            {
                RampCoordinates flippedCoords;
                flippedCoords.coord1 = new Vector3Int(rampCoordinates[i].coord1.z, rampCoordinates[i].coord1.y, rampCoordinates[i].coord1.x);
                flippedCoords.coord2 = new Vector3Int(rampCoordinates[i].coord2.z, rampCoordinates[i].coord2.y, rampCoordinates[i].coord2.x);
                rampCoordinates[i] = flippedCoords;
            }
        }
    }

    private void FlipDiagonal(ref List<Vector3Int> coordinates)
    {
        if (coordinates.Count > 0)
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i] = new Vector3Int(coordinates[i].z,coordinates[i].y,coordinates[i].x);
            }
        }
    }

    private void FlipX(ref List<List<Vector3Int>> patrolRoutes)
    {
        if (patrolRoutes.Count > 0)
        {
            for (int i = 0; i < patrolRoutes.Count; i++)
            {
                if (patrolRoutes[i].Count > 0)
                {
                    for (int j = 0; j < patrolRoutes[i].Count; j++)
                    {
                        patrolRoutes[i][j] += new Vector3Int(1, 0, 0) * (roomMaxX - patrolRoutes[i][j].x * 2);
                    }
                }
            }
        }
    }

    private void FlipX(ref List<RampCoordinates> rampCoordinates)
    {
        if (rampCoordinates.Count > 0)
        {
            for (int i = 0; i < rampCoordinates.Count; i++)
            {
                RampCoordinates flippedCoords;
                flippedCoords.coord1 = new Vector3Int(roomMaxX - rampCoordinates[i].coord1.x,rampCoordinates[i].coord1.y,rampCoordinates[i].coord1.z);
                flippedCoords.coord2 = new Vector3Int(roomMaxX - rampCoordinates[i].coord2.x, rampCoordinates[i].coord2.y, rampCoordinates[i].coord2.z);
                rampCoordinates[i] = flippedCoords;
            }
        }
    }

    private void FlipX(ref List<Vector3Int> coordinates)
    {
        if (coordinates.Count > 0)
        {
            for (int i=0; i < coordinates.Count; i++)
            {
                coordinates[i] += Vector3Int.right * (roomMaxX - coordinates[i].x * 2);
            }
        }
    }

    internal Orientation GetOrientation(Vector3Int exit)
    {
        if(exit.x==roomMaxX)
        {
            return Orientation.right;
        }
        if(exit.x==roomMinX)
        {
            return Orientation.left;
        }
        if(exit.z==roomMaxZ)
        {
            return Orientation.forward;
        }
        return Orientation.back;
    }

    internal Vector3Int GetRandomExit()
    {
        Vector3Int exit = exits[Random.Range(0, exits.Count)];
        exits.Remove(exit);
        return exit;
    }

    private void ConfigureExitHeights()
    {
        foreach(Vector3Int tile in tiles)
        {
            for(int i=0;i<exits.Count;i++)
            {
                if(tile.x==exits[i].x&&tile.z==exits[i].z)
                {
                    exits[i] += Vector3Int.up * tile.y;
                }
            }
        }
    }

    private void ParseRoomFile(string roomText)
    {
        string nextLine = GetNextLine(ref roomText);
        switch (nextLine)
        {
            case "++LAYOUT++":
                {
                    ParseLayout(ref roomText);
                    break;
                }
            case "++PLACEMENTS++":
                {
                    ParsePlacements(ref roomText);
                    break;
                }
            case "++PATROL ROUTES++":
                {
                    ParseRoutes(ref roomText);
                    break;
                }
            case "++TERMINAL ASSIGNMENTS++":
                {
                    ParseAssignments(ref roomText);
                    break;
                }
            case "++LOOT VALUES++":
                {
                    ParseLootValues(ref roomText);
                    break;
                }
            case "++ENDFILE++":
                {
                    return;
                }
        }
    }

    private void ParseAssignments(ref string roomText)
    {
        terminalAssignments = new List<TerminalAssignment>();
        isControlledByInternalTerminal = true;
        string nextLine = GetNextLine(ref roomText);
        while (nextLine!="++END++")
        {
            int newVal = 0;
            int x = 0;
            int z = 0;
            TerminalAssignment newAssignement = new TerminalAssignment();
            foreach (char nextChar in nextLine)
            {
                if (Char.IsDigit(nextChar))
                {
                    newVal *= 10;
                    newVal += int.Parse(nextChar.ToString());
                }
                else if (nextChar == ',')
                {
                    x = newVal;
                    newVal = 0;
                }
                else if (nextChar == '-')
                {
                    z = newVal;
                    newVal = 0;
                    newAssignement.AddLocation(new Vector3Int(x, 0, z));
                }
            }
            z = newVal;
            newAssignement.AddLocation(new Vector3Int(x, 0, z));
            terminalAssignments.Add(newAssignement);
            nextLine = GetNextLine(ref roomText);
        }
        ParseRoomFile(roomText);
    }

    private void ParseLootValues(ref string roomText)
    {
        string nextLine = GetNextLine(ref roomText);
        while (nextLine!="++END++")
        {
            int newVal = 0;
            int x=0;
            int z=0;
            int lootValue;
            foreach(char nextChar in nextLine)
            {
                if (Char.IsDigit(nextChar))
                {
                    newVal *= 10;
                    newVal += int.Parse(nextChar.ToString());
                }
                else if (nextChar == ',')
                {
                    x = newVal;
                    newVal = 0;
                }
                else if (nextChar == '-')
                {
                    z = newVal;
                    newVal = 0;
                }
            }
            lootValue = newVal;
            if(missionObj!=Vector3Int.down&&missionObj.x==x&&missionObj.z==z)
            {
                missionObj += Vector3Int.up * lootValue;
            }
            else for(int i=0;i<loot.Count;i++)
            {
                if(loot[i].x == x && loot[i].z == z)
                {
                    loot[i]+= Vector3Int.up*lootValue;
                }
            }
            nextLine = GetNextLine(ref roomText);
        }
        ParseRoomFile(roomText);
    }

    private void ParseRoutes(ref string roomText)
    {
        string nextLine = GetNextLine(ref roomText);
        while (nextLine != "++END++")
        {
            List<Vector3Int> patrolPoints = new List<Vector3Int>();
            int newVal = 0;
            int x = 0;
            int z = 0;
            Vector3Int newPoint;
            foreach (char nextChar in nextLine)
            {
                if (Char.IsDigit(nextChar))
                {
                    newVal *= 10;
                    newVal += int.Parse(nextChar.ToString());
                }
                else if (nextChar == ',')
                {
                    x = newVal;
                    newVal = 0;
                }
                else if (nextChar == '-')
                {
                    z = newVal;
                    newVal = 0;
                    newPoint = new Vector3Int(x,0,z);
                    patrolPoints.Add(newPoint);
                }
            }
            z = newVal;
            newVal = 0;
            newPoint = new Vector3Int(x, 0, z);
            patrolPoints.Add(newPoint);
            patrolRoutes.Add(patrolPoints);
            nextLine = GetNextLine(ref roomText);
        }
        ParseRoomFile(roomText);
    }

    private void ParsePlacements(ref string roomText)
    {
        string nextLine = GetNextLine(ref roomText);
        while (nextLine != "++END++")
        {
            int x = 0;
            int z = 0;
            while (nextLine != "++END++")
            {
                foreach (char nextChar in nextLine)
                {
                    switch (nextChar)
                    {
                        case ' ':
                            {
                                x++;
                                break;
                            }
                        case '-':
                            {
                                break;
                            }
                        case 'S':
                            {
                                deploymentPoint = new Vector3Int(x, 0, z);
                                break;
                            }
                        case 'E':
                            {
                                enemies.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'R':
                            {
                                RampCoordinates newCoords;
                                newCoords.coord1 = new Vector3Int(x, 0, z);
                                newCoords.coord2 = FindRampTop(x, z);
                                rampCoordinates.Add(newCoords);
                                break;
                            }
                        case 'U':
                            {
                                defences.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'H':
                            {
                                securityHubs.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'L':
                            {
                                loot.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'O':
                            {
                                firewalls.Add(new Vector3Int(x, 0, z));
                                missionObj = new Vector3Int(x, 0, z);
                                break;
                            }
                        case 'V':
                            {
                                loot.Add(new Vector3Int(x, 0, z));
                                firewalls.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'T':
                            {
                                terminals.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'P':
                            {
                                ports.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'F':
                            {
                                firewalls.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                        case 'I':
                            {
                                entrance = new Vector3Int(x, 0, z);
                                break;
                            }
                        case 'X':
                            {
                                exits.Add(new Vector3Int(x, 0, z));
                                break;
                            }
                    }
                }
                z++;
                x = 0;
                nextLine = GetNextLine(ref roomText);
            }
        }
        ParseRoomFile(roomText);
    }

    private Vector3Int FindRampTop(int x, int z)
    //for a ramp at given coordinates, determine which direction is the top of the ramp
    {
        int y = GetTileHeight(x,z); 
        foreach (Vector3Int tile in tiles)
        {
            if(((tile.x==x&&Math.Abs(tile.z-z)==1)||(tile.z == z && Math.Abs(tile.x - x) == 1)) // are tiles adjacent
                && tile.y ==y+1)
            {
                return tile;
            }
        }
        return Vector3Int.zero;
    }

    private int GetTileHeight(int x, int z)
    {
        foreach (Vector3Int tile in tiles)
        {
            if (tile.x == x && tile.z == z) // find height of tile with ramp
            {
                return tile.y;
            }
        }
        return 0;
    }

    private void ParseLayout(ref string roomText)
    {
        string nextLine = GetNextLine(ref roomText);
        int x = 0;
        int z = 0;
        while (nextLine != "++END++")
        {
            foreach(char nextChar in nextLine)
            {
                switch(nextChar)
                {
                    case ' ':
                        {
                            x++;
                            break;
                        }
                    case '-':
                        {
                            break;
                        }
                    default:
                        {
                            if (Char.IsDigit(nextChar))
                            {
                                tiles.Add(new Vector3Int(x, int.Parse(nextChar.ToString()), z));
                            }
                            break;
                        }
                }
            }
            x = 0;
            z++;
            nextLine = GetNextLine(ref roomText);
        }
        ParseRoomFile(roomText);
    }

    private static string GetNextLine(ref string roomText)
    {
        int indexOfNextNewline = roomText.IndexOf("\n");
        string nextLine = roomText.Substring(0, indexOfNextNewline-1);
        roomText = roomText.Substring(indexOfNextNewline + 1);
        return nextLine;
    }

    internal void AdjustCoordinatesToMinimizedGrid(int minX,int minZ)
    {
        for(int i=0;i<tiles.Count;i++)
        {
            tiles[i] += new Vector3Int(-1, 0, 0)*minX+ new Vector3Int(0,0,-1)*minZ;
        }
        if (rampCoordinates.Count > 0)
        {
            for (int i = 0; i < rampCoordinates.Count; i++)
            {
                RampCoordinates newCoords;
                newCoords.coord1 = rampCoordinates[i].coord1 + new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
                newCoords.coord2 = rampCoordinates[i].coord2 + new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
                rampCoordinates[i] = newCoords;
            }
        }
        if (firewalls.Count > 0)
        {
            for (int i = 0; i < firewalls.Count; i++)
            {
                firewalls[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (terminals.Count > 0)
        {
            for (int i = 0; i < terminals.Count; i++)
            {
                terminals[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (ports.Count > 0)
        {
            for (int i = 0; i < ports.Count; i++)
            {
                ports[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (loot.Count > 0)
        {
            for (int i = 0; i < loot.Count; i++)
            {
                loot[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (securityHubs.Count > 0)
        {
            for (int i = 0; i < securityHubs.Count; i++)
            {
                securityHubs[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (defences.Count > 0)
        {
            for (int i = 0; i < defences.Count; i++)
            {
                defences[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i] += new Vector3Int(-1, 0, 0) * minX + new Vector3Int(0, 0, -1) * minZ;
            }
        }
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
}
