using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonDirectory : MonoBehaviour
{
    [SerializeField] string[] corpDungeonNames;

    internal void LoadCorpDungeon(int corpID)
    {
        if(corpID>=0&&corpID<corpDungeonNames.Length)
        {
            SceneManager.LoadScene(corpDungeonNames[corpID]);
        }
        else
        {
            SceneManager.LoadScene("procDungeon");
        }
    }
}
