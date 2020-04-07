using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plugin : MonoBehaviour
{
    [SerializeField] internal int size;
    [SerializeField] internal int power;
    [SerializeField] internal int speed;
    [SerializeField] internal int range;
    [SerializeField] internal int sight;
    [SerializeField] internal int breach;
    [SerializeField] internal List<string> keywords;
    [SerializeField] internal string proc;

    [SerializeField] internal Sprite icon;

    internal void AttemptAddToPackage(int programRecievingPluginIndex)
    {
        MissionStatus missionPrep = MissionStatus.instance;
        if (missionPrep.selectedPrograms[programRecievingPluginIndex])
        {
            PlayerProgram programRecievingPlugin = missionPrep.selectedPrograms[programRecievingPluginIndex].GetComponent<PlayerProgram>();
            if (CanAddPluginToProgram(programRecievingPluginIndex, programRecievingPlugin))
            {
                missionPrep.selectedPlugins[programRecievingPluginIndex].Add(gameObject);
                missionPrep.kbBudget -= size;
                missionPrep.SetPlugin(programRecievingPluginIndex, this);
            }
        }
    }

    private bool CanAddPluginToProgram(int programRecievingPluginIndex, PlayerProgram programRecievingPlugin)
    {
        return MissionStatus.instance.kbBudget >= size &&
                    programRecievingPlugin.pluginSlots > MissionStatus.instance.selectedPlugins[programRecievingPluginIndex].Count &&
                    !MissionStatus.instance.selectedPlugins[programRecievingPluginIndex].Contains(gameObject);
    }

    internal void RemoveFromPackage(int programToRemoveFromIndex)
    {
        MissionStatus missionPrep = MissionStatus.instance;
        missionPrep.selectedPlugins[programToRemoveFromIndex].Remove(gameObject);
        missionPrep.kbBudget += size;
        missionPrep.ResetPluginIcon(programToRemoveFromIndex,this.name);
    }
}
