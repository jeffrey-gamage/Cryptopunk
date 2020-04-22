using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomDirectory : MonoBehaviour
{
    //container for text room layouts
    //to be prefabbed once for each corp
    public TextAsset[] STARTING_ROOMS;
    public TextAsset[] GENERAL_ROOMS;
    public TextAsset[] FINAL_ROOMS;

    [SerializeField] int corpID;

    internal int GetFirstRoomIndex()
    {
        return Random.Range(0, STARTING_ROOMS.Length);
    }

    internal int[] GetGenRoomIndices(int numRooms)
    {
        int[] genRoomIndices = new int[numRooms];
        List<int> availableIndices = new List<int>();
        if (GENERAL_ROOMS.Length > 0)
        {
            for (int i = 0; i < GENERAL_ROOMS.Length; i++)
            {
                availableIndices.Add(i);
            }
            for (int i = 0; i < genRoomIndices.Length; i++)
            {
                int selectedIndex = Random.Range(0, availableIndices.Count);
                genRoomIndices[i] = availableIndices[selectedIndex];
                availableIndices.RemoveAt(selectedIndex);
            }
        }
        return genRoomIndices;
    }

    internal static RoomDirectory GetInstance()
    {
        return FindObjectOfType<RoomDirectory>();
    }

    internal int GetFinalRoomIndex()
    {
        List<int> myCorpUsedFinalRooms = new List<int>();
        foreach(PersistentState.UsedFinalRoomRecord record in PersistentState.instance.usedFinalRoomRecords)
        {
            if(record.corpID==corpID)
            {
                myCorpUsedFinalRooms.Add(record.roomIndex);
            }
        }
        if(PersistentState.instance.usedFinalRoomRecords.Count>=FINAL_ROOMS.Length)
        {
            PersistentState.instance.usedFinalRoomRecords.Clear();
        }
        while(true)
        {
            int finalRoomIndex = Random.Range(0, FINAL_ROOMS.Length);
            if(!myCorpUsedFinalRooms.Contains(finalRoomIndex))
            {
                PersistentState.UsedFinalRoomRecord newRecord;
                newRecord.corpID = corpID;
                newRecord.roomIndex = finalRoomIndex;
                PersistentState.instance.usedFinalRoomRecords.Add(newRecord);
                return finalRoomIndex;
            }
        }
    }
}
