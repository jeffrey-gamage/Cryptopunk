using System.Collections;
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
    Text attackButtonText;
    Text breachButtonText;
    // Start is called before the first frame update
    void Start()
    {
        attackButtonText = attackButton.GetComponentInChildren<Text>();
        breachButtonText = breachButton.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Program.selectedProgram)
        {
            nameDisplay.text = Program.selectedProgram.name;
            sizeDisplay.text = Program.selectedProgram.size.ToString() + " / " + Program.selectedProgram.GetSize().ToString();
            speedDisplay.text = Program.selectedProgram.movesLeft.ToString() + " / " + Program.selectedProgram.GetSpeed().ToString();
            powerDisplay.text = Program.selectedProgram.GetPower().ToString();
            rangeDisplay.text = Program.selectedProgram.GetRange().ToString();
            if (DungeonManager.instance.IsPlayers(Program.selectedProgram))
            {
                breachLabel.text = "Breach";
                breachDisplay.text = Program.selectedProgram.GetBreach().ToString();
            }
            else
            {
                breachLabel.text = "Integrity";
                breachDisplay.text = Program.selectedProgram.gameObject.GetComponent<Hackable>().currentIntegrity.ToString() + " / " + Program.selectedProgram.gameObject.GetComponent<Hackable>().maxIntegrity.ToString();
            }
            sightDisplay.text = Program.selectedProgram.GetSight().ToString();
            string keywords = "";
            foreach(string keyword in Program.selectedProgram.GetKeywords())
            {
                keywords += keyword + "\n";
            }
            keywordDisplay.text = keywords;

            attackButton.enabled = Program.selectedProgram.IsControlledByPlayer() &&
                Program.selectedProgram.GetPower() > 0 &&
                !Program.selectedProgram.hasAttacked;
            breachButton.enabled= Program.selectedProgram.IsControlledByPlayer() &&
                Program.selectedProgram.GetBreach() > 0 &&
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
        if(DungeonManager.instance.mode==DungeonManager.Mode.Attack)
        {
            attackButtonText.text = "Cancel Attack";
        }
        else
        {
            attackButtonText.text = "Attack";
        }
        if (DungeonManager.instance.mode == DungeonManager.Mode.Breach)
        {
            breachButtonText.text = "Cancel Breach";
        }
        else
        {
            breachButtonText.text = "Breach";
        }
    }
    public void Target()
    {
        if (DungeonManager.instance.mode == DungeonManager.Mode.Attack)
        {
            CancelTargeting();
        }
        else
        {
            Program.isTargetingAttack = true;
            DungeonManager.instance.mode = DungeonManager.Mode.Attack;
        }
    }
    public void TargetBreach()
    {
        if (DungeonManager.instance.mode == DungeonManager.Mode.Breach)
        {
            CancelTargeting();
        }
        else
        {
            Program.isTargetingBreach = true;
            DungeonManager.instance.mode = DungeonManager.Mode.Breach;
        }
    }
    private void CancelTargeting()
    {
        Program.isTargetingAttack = false;
        Program.isTargetingBreach = false;
        DungeonManager.instance.mode = DungeonManager.Mode.Move;
    }
}
