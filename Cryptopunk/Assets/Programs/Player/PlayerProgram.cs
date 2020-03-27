using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgram : Program
{
    [SerializeField] internal int pluginSlots;
    [SerializeField] Material stealthMaterial;
    [SerializeField] float iconStealthAlpha = 0.3f;
    private Color visibleColor;
    private Color stealthColor;

    internal override void Start()
    {
        base.Start();
        visibleColor = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, 1);
        stealthColor = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, iconStealthAlpha);
    }

    override internal void Update()
    {
        base.Update();
        if (updateStealthVisuals)
        {
            ShowStealthVisuals();
            updateStealthVisuals = false;
        }
    }
    private void ShowStealthVisuals()
    {
        if (myIcon)
        {
            myIcon.enabled = myRenderer.enabled;
            if (IsStealthed())
            {
                myIcon.color = stealthColor;
            }
            else
            {
                myIcon.color = visibleColor;
            }
        }
        if (IsStealthed())
        {
            myRenderer.material = stealthMaterial;
        }
        else
        {
            myRenderer.material = standardMaterial;
        }
    }

}
