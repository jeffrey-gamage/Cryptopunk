﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramDisplay : MonoBehaviour
{
    [SerializeField] Text nameDisplay;
    [SerializeField] Text sizeDisplay;
    [SerializeField] Text speedDisplay;
    [SerializeField] Text powerDisplay;
    [SerializeField] Text rangeDisplay;
    [SerializeField] Text breachLabel;
    [SerializeField] Text breachDisplay;
    [SerializeField] Text sightDisplay;
    [SerializeField] Text keywordDisplay;
    [SerializeField] Button attackButton;
    [SerializeField] Button breachButton;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Program.selectedProgram)
        {
            nameDisplay.text = Program.selectedProgram.name;
            sizeDisplay.text = Program.selectedProgram.size.ToString() + " / " + Program.selectedProgram.maxSize.ToString();
            speedDisplay.text = Program.selectedProgram.movesLeft.ToString() + " / " + Program.selectedProgram.speed.ToString();
            powerDisplay.text = Program.selectedProgram.power.ToString();
            rangeDisplay.text = Program.selectedProgram.range.ToString();
            if (DungeonManager.instance.IsPlayers(Program.selectedProgram))
            {
                breachLabel.text = "Breach";
                breachDisplay.text = Program.selectedProgram.breach.ToString();
            }
            else
            {
                breachLabel.text = "Integrity";
                breachDisplay.text = Program.selectedProgram.gameObject.GetComponent<Hackable>().currentIntegrity.ToString() + " / " + Program.selectedProgram.gameObject.GetComponent<Hackable>().maxIntegrity.ToString();
            }
            sightDisplay.text = Program.selectedProgram.sight.ToString();
            string keywords = "";
            foreach(string keyword in Program.selectedProgram.keywords)
            {
                keywords += keyword + "\n";
            }
            keywordDisplay.text = keywords;

            attackButton.enabled = DungeonManager.instance.IsPlayers(Program.selectedProgram) &&
                Program.selectedProgram.power > 0 &&
                !Program.selectedProgram.hasAttacked;
            breachButton.enabled= DungeonManager.instance.IsPlayers(Program.selectedProgram) &&
                Program.selectedProgram.breach > 0 &&
                !Program.selectedProgram.hasAttacked;
        }
        else
        {
            nameDisplay.text = "";
            sizeDisplay.text = "";
            speedDisplay.text = "";
            powerDisplay.text = "";
            rangeDisplay.text = "";
            breachDisplay.text = "";
            sightDisplay.text = "";
            keywordDisplay.text = "";
            attackButton.enabled = false;
            breachButton.enabled = false;

        }
    }
    public void Target()
    {
        Program.isTargeting = true;
    }
}
