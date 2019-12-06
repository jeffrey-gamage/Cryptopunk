using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    public static Hackable selectedObject;
    [SerializeField] internal int maxIntegrity;
    [SerializeField] int rebootTime = 3;
    internal int currentIntegrity;
    internal DungeonTile myTile;
    private bool isHacked;
    internal bool isEnabled = true;
    internal Program myProgram;
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
                Activate();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentIntegrity = maxIntegrity;
        myProgram = GetComponent<Program>();
    }
    private void OnMouseDown()
    {
        if(Program.isTargetingBreach&&Program.selectedProgram&&Program.selectedProgram.IsControlledByPlayer())
        {
            Program.selectedProgram.AttemptBreach(this);
        }
        else
        {
            Program.selectedProgram = myProgram;
            selectedObject = this;
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
            Deactivate();
        }
    }
    internal void SetTile(DungeonTile tile)
    {
        myTile = tile;
        if (isEnabled)
        {
            tile.isBlocked = true;
        }
    }
    internal virtual void Activate()
    {
        //hook for subclasses to do behaviour when switched on
        isEnabled = true;
    }
    internal virtual void Deactivate()
    {
        //hook for subclasses to do behaviour when switched off
        isEnabled = false;
    }

    internal bool IsHacked()
    {
        return isHacked;
    }
}
