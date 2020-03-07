using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo: RoomDirectory
{

    [SerializeField] Vector2Int[] terminalControlAssignments; //x = index of terminal in terminals list, y= index of controlled object in object list
    [SerializeField] Vector3Int[] enemyInfo;
    [SerializeField] internal GameObject[] tutorialPrograms;

    internal Vector3Int[] GetEnemies()
    {
        return enemyInfo;
    }
}