using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramMenuItem : MonoBehaviour
{
    private GameObject myProgramPrefab;

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
        myProgramPrefab = programPrefab;
        Program program = myProgramPrefab.GetComponent<Program>();
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
        icon.sprite = myProgramPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public void AddToPackage()
    {
        FindObjectOfType<MissionStatus>().SubmitProgramToPackage(myProgramPrefab);
    }
}
