using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonDirectory : MonoBehaviour
{
    [SerializeField] string[] corpDungeonNames;
    [SerializeField] int[] unbuiltDungeons;

    internal void LoadCorpDungeon(int corpID)
    {
        if(corpID>=0&&corpID<corpDungeonNames.Length&&IsBuilt(corpID))
        {
            SceneManager.LoadScene(corpDungeonNames[corpID]);
        }
        else
        {
            SceneManager.LoadScene("roomTester");
        }
    }

    private bool IsBuilt(int corpID)
    {
        foreach(int dungeonID in unbuiltDungeons)
        {
            if (dungeonID==corpID)
            {
                return false;
            }
        }
        return true;
    }
}
