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
        if(IsHacked()&&!isEnabled&&!FindObjectOfType<PortUI>())
        {
            PortUI newPortUI =Instantiate(portUI, FindObjectOfType<Canvas>().transform).GetComponent<PortUI>();
            newPortUI.SetPort(this);
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

    internal void Import(GameObject program)
    {
        rebootCountdown = program.GetComponent<Program>().maxSize;
        DungeonManager.instance.DeployFromPort(myTile,program);
    }
}
