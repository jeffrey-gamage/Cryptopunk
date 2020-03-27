using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPrepMenuItem : MonoBehaviour
{
    private GameObject myPrefab;

    [SerializeField] Image icon;
    [SerializeField] Text programName;
    [SerializeField] Text programSize;
    [SerializeField] Text programSpeed;
    [SerializeField] Text programSight;
    [SerializeField] Text programRange;
    [SerializeField] Text programPower;
    [SerializeField] Text programBreach;
    [SerializeField] Text programKeywords;

    internal void SetProgram(GameObject programPrefab)
    {
        myPrefab = programPrefab;
        Program program = myPrefab.GetComponent<Program>();
        programName.text = program.name;
        programSize.text = "Size: " + program.GetSize().ToString();
        programSpeed.text = "Speed: " + program.GetSpeed().ToString();
        programSight.text = "Sight: " + program.GetSight().ToString();
        programRange.text = "Range: " + program.GetRange().ToString();
        programPower.text = "Power: " + program.GetPower().ToString();
        programBreach.text = "Breach: " + program.GetBreach().ToString();
        programKeywords.text = "Abilities: \n";
        foreach (string keyword in program.GetKeywords())
        {
            programKeywords.text += "  " + keyword + "\n";
        }
        icon.sprite = myPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
    }
    internal void SetPlugin(GameObject pluginPrefab)
    {
        myPrefab = pluginPrefab;
        Plugin plugin = myPrefab.GetComponent<Plugin>();

        icon.sprite = plugin.icon;
        programName.text = plugin.name;
        programSize.text = "size: +" + plugin.size.ToString();
        programSpeed.text = "speed: +" + plugin.speed.ToString();
        programSight.text = "sight: +" + plugin.sight.ToString();
        programPower.text = "power: +" + plugin.power.ToString();
        programRange.text = "range: +" + plugin.range.ToString();
        programBreach.text = "breach: +" + plugin.breach.ToString();
        programKeywords.text = "";
        foreach (string keyword in plugin.keywords)
        {
            programKeywords.text += keyword + "\n";
        }
    }

    public void AddToPackage()
    {
        if (myPrefab.GetComponent<Program>())
        {
            FindObjectOfType<MissionStatus>().SubmitProgramToPackage(myPrefab);
        }
        else
        {
            FindObjectOfType<MissionStatus>().AttemptInstallPlugin(myPrefab);
        }
    }
}
