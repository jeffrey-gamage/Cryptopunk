using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : DungeonTile
    //tiles that can be toggled on and off by switchBridge. Should not have any loot or objects other than programs placed on them.
{
    [SerializeField] float fadeRate = 0.6f;

    [SerializeField] internal bool isOn;
    internal void Switch(bool on)
    {
        isOn = on;
    }

    protected override void Update()
    {
        base.Update();
        if(!isOn)
        {
            isBlocked = true;
            if (myMeshRenderer.material.color.a > 0f)
                myMeshRenderer.material.color = new Color(myMeshRenderer.material.color.r, myMeshRenderer.material.color.g, myMeshRenderer.material.color.b,
                    Mathf.Max(0f, myMeshRenderer.material.color.a - fadeRate * Time.deltaTime));
        }
        else
        {
            isBlocked = isOccupied;
            if (myMeshRenderer.material.color.a < 1f)
                    myMeshRenderer.material.color = new Color(myMeshRenderer.material.color.r, myMeshRenderer.material.color.g, myMeshRenderer.material.color.b,
                        Mathf.Min(0f, myMeshRenderer.material.color.a + fadeRate * Time.deltaTime));
        }
    }
}
