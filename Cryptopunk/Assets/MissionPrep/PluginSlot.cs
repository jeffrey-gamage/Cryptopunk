using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PluginSlot : MonoBehaviour
{
    internal int myProgramSlotIndex;
    internal Plugin selectedPlugin;
    [SerializeField] internal Sprite emptySprite;

    internal void ResetPlugin()
    {
        selectedPlugin = null;
        GetComponent<Image>().sprite = emptySprite;
    }

    internal bool IsEmpty()
    {
        return selectedPlugin==null;
    }

    internal void SetPlugin(Plugin plugin)
    {
        if(!plugin)
        {
            ResetPlugin();
        }
        else
        {
            selectedPlugin = plugin;
            GetComponent<Image>().sprite = plugin.icon;
        }
    }

    public void RemoveMyPlugin()
    {
        if (selectedPlugin)
        {
            selectedPlugin.RemoveFromPackage(myProgramSlotIndex);
            ResetPlugin();
        }
    }
}
