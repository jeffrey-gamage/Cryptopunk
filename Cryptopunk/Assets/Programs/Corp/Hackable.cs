using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    [SerializeField] internal int maxIntegrity;
    [SerializeField] int rebootTime = 3;
    internal int currentIntegrity;
    private Program hacker;
    internal int rebootCountdown=0;

    internal void OnStartTurn()
    {
        if(hacker)
        {
            rebootCountdown--;
            if(rebootCountdown<=0)
            {
                rebootCountdown = rebootTime;
                currentIntegrity = maxIntegrity;
                hacker = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentIntegrity = maxIntegrity;
    }

    internal void Disrupt(int damageAmount)
    {
        currentIntegrity = Mathf.Clamp(currentIntegrity - damageAmount, 1, maxIntegrity);
    }

    internal void Breach(int breachAmount, Program hacker)
    {
        currentIntegrity = Mathf.Clamp(currentIntegrity - breachAmount, 0, maxIntegrity);
        if(currentIntegrity<=0)
        {
            this.hacker = hacker;
            rebootCountdown = rebootTime;
        }
    }
}
