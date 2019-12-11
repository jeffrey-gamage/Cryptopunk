using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramListItem : MonoBehaviour
{
    [SerializeField] Text programName;
    [SerializeField] Text programSize;
    [SerializeField] Text programSpeed;
    [SerializeField] Text programSight;
    [SerializeField] Text programRange;
    [SerializeField] Text programPower;
    [SerializeField] Text programBreach;
    [SerializeField] Text programKeywords;
    private GameObject myProgram;

    private void OnMouseDown()
    {
        FindObjectOfType<PortUI>().Select(myProgram.GetComponent<Program>());
    }

    internal void SetProgram(GameObject newProgram)
    {
        myProgram = newProgram;
        Program program = myProgram.GetComponent<Program>();
        programName.text = program.name;
        programSize.text = "Size: "+program.maxSize.ToString();
        programSpeed.text = "Speed: " + program.speed.ToString();
        programSight.text = "Sight: " + program.sight.ToString();
        programRange.text = "Range: " + program.range.ToString();
        programPower.text = "Power: " + program.power.ToString();
        programBreach.text = "Breach: " + program.breach.ToString();
        programKeywords.text = "Abilities: \n";
        foreach(string keyword in program.keywords)
        {
            programKeywords.text += "  " + keyword + "\n";
        }
    }
}
