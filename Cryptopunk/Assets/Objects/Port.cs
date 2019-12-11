using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : Hackable
{
    [SerializeField] GameObject portUI;
    private bool isOpen = false;
    internal override void Activate()
    {
        if(IsHacked()&&!isEnabled)
        {
            Instantiate(portUI, FindObjectOfType<Canvas>().transform);
        }
        base.Activate();
    }

    internal override void Breach(int breachAmount)
    {
        isOpen = true;
        base.Breach(breachAmount);
    }

    internal override void Deactivate()
    {
        if (isOpen)
        {
            base.Deactivate();
            isOpen = false;
        }
    }

    internal void Import(Program program)
    {
        DungeonManager.instance.DeployFromPort(myTile,program);
    }
}
