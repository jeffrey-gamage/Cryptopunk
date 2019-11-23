using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    [SerializeField] internal int maxIntegrity;
    [SerializeField] int rebootTime = 3;
    internal int currentIntegrity;
    internal DungeonTile myTile;
    private bool isHacked;
    internal int rebootCountdown=0;

    internal void OnStartTurn()
    {
        if(isHacked)
        {
            rebootCountdown--;
            if(rebootCountdown<=0)
            {
                rebootCountdown = rebootTime;
                currentIntegrity = maxIntegrity;
                isHacked = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentIntegrity = maxIntegrity;
    }
    private void OnMouseDown()
    {
        if(Program.isTargetingBreach&&Program.selectedProgram.IsControlledByActivePlayer())
        {
            Program.selectedProgram.AttemptBreach(this);
        }
    }
    internal void Disrupt(int damageAmount)
    {
        currentIntegrity = Mathf.Clamp(currentIntegrity - damageAmount, 1, maxIntegrity);
    }

    internal void Breach(int breachAmount, Program hacker)
    {
        currentIntegrity -= breachAmount;
        if(currentIntegrity<=0)
        {
            rebootCountdown = rebootTime;
            if (GetComponent<EnemyProgram>())
            {
                GetComponent<EnemyProgram>().OnStartTurn();
                rebootCountdown++;
                currentIntegrity = 0;
            }
            this.isHacked = true;
        }
    }

    internal bool IsHacked()
    {
        return isHacked;
    }
}
