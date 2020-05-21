using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : Hackable
{
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite hackedSprite;
    [SerializeField] SpriteRenderer face;

    [SerializeField] GameObject portUI;
    private bool isOpen = false;
    internal override void Activate()
    {
        if(IsHacked()&&!isEnabled&&!FindObjectOfType<PortUI>())
        {
            PortUI newPortUI =Instantiate(portUI, FindObjectOfType<Canvas>().transform).GetComponent<PortUI>();
            newPortUI.SetPort(this);
            face.sprite = normalSprite;
        }
        base.Activate();
    }

    internal override void Breach(int breachAmount)
    {
        isOpen = true;
        base.Breach(breachAmount);
        face.sprite = hackedSprite;
    }

    internal override void Deactivate(bool isBreach)
    {
        if (isOpen)
        {
            base.Deactivate(isBreach);
            isOpen = false;
        }
    }

    internal void Import(GameObject program)
    {
        rebootCountdown = program.GetComponent<Program>().GetSize();
        DungeonManager.instance.DeployFromPort(myTile,program);
    }
}
