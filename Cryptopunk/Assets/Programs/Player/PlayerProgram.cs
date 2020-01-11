using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgram : Program
{
    [SerializeField] Material stealthMaterial;
    [SerializeField] float iconStealthAlpha = 0.3f;

    override internal void Update()
    {
        base.Update();
        ShowStealthVisuals();
    }
    private void ShowStealthVisuals()
    {
        if (myIcon)
        {
            myIcon.enabled = myRenderer.enabled;
            if (IsStealthed())
            {
                myIcon.color = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, iconStealthAlpha);
            }
            else
            {
                myIcon.color = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, 1);
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
