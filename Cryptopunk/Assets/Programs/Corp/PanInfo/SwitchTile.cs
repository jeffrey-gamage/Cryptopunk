using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : DungeonTile
    //tiles that can be toggled on and off by switchBridge. Should not have any loot or objects other than programs placed on them.
{
    [SerializeField] float fadeRate = 0.6f;
    private float opacity = 0f;

    [SerializeField] internal bool isOn;
    internal void Switch(bool on)
    {
        isOn = on;
    }

    protected override void Update()
    {
        base.Update();
        if (!isOn)
        {
            isBlocked = true;
            if (opacity > 0f)
            {
                opacity = Mathf.Max(0f, opacity - fadeRate * Time.deltaTime);
            }
            else if(IsExplored())
            {
                myMeshRenderer.enabled = false;
            }
        }
        else
        {
            isBlocked = isOccupied;
            if (opacity < 1f)
            {
                opacity = Mathf.Min(1f, opacity + fadeRate * Time.deltaTime);
            }
            else if (IsExplored())
            {
                myMeshRenderer.enabled = true;
            }
        }
        if (IsExplored())
        {
            myMeshRenderer.material.color = new Color(myMeshRenderer.material.color.r, myMeshRenderer.material.color.g, myMeshRenderer.material.color.b, opacity);
        }
    }
}
