using System;
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

    [SerializeField] Button[] procButtons;
    private List<Procable> procPlugins;

    private bool hadTargetLastFrame = true;
    // Start is called before the first frame update
    void Start()
    {
        attackButtonText = attackButton.GetComponentInChildren<Text>();
        breachButtonText = breachButton.GetComponentInChildren<Text>();
        procPlugins = new List<Procable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Program.selectedProgram)
        {
            if (!hadTargetLastFrame)
            {
                SwitchPanelVisibility(true);
            }
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
            UpdateProcButtons();
        }
        else
        {
            if(hadTargetLastFrame)
            {
                SwitchPanelVisibility(false);
            }
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

    private void UpdateProcButtons()
    {
        procPlugins.Clear();
        int procButtonIndex = 0;
        int pluginIndex = 0;
        while(pluginIndex<Program.selectedProgram.plugins.Count)
        {
            if(Program.selectedProgram.plugins[pluginIndex].proc.Length>0)
            {
                procPlugins.Add((Procable)Program.selectedProgram.plugins[pluginIndex]);
                SetProcButtonTargetAndVisibility(procButtons[procButtonIndex],Program.selectedProgram.plugins[pluginIndex].proc);
                procButtons[procButtonIndex].enabled = Program.selectedProgram.IsControlledByPlayer() && !Program.selectedProgram.hasAttacked;
                procButtonIndex++;
            }
            pluginIndex++;
        }
        while(procButtonIndex<procButtons.Length)
        {
            SetProcButtonTargetAndVisibility(procButtons[procButtonIndex], "");
            procButtonIndex++;
        }
    }

    private void SetProcButtonTargetAndVisibility(Button button, string proc)
    {
        button.enabled = proc.Length > 0;
        button.GetComponent<Image>().enabled = proc.Length > 0;
        button.GetComponentInChildren<Text>().enabled = proc.Length > 0;
        if (DungeonManager.instance.mode != DungeonManager.Mode.Proc)
        {
            button.GetComponentInChildren<Text>().text = proc;
        }
        else
        {
            button.GetComponentInChildren<Text>().text = "Cancel";
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
            Program.isTargetingBreach = false;
            Program.isTargetingProc = false;
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
            Program.isTargetingAttack = false;
            Program.isTargetingProc = false;
            DungeonManager.instance.mode = DungeonManager.Mode.Breach;
        }
    }
    public void TargetProc(int procIndex)
    {
        if(DungeonManager.instance.mode == DungeonManager.Mode.Proc)
        {
            CancelTargeting();
        }
        else
        {
            Program.isTargetingProc = true;
            Program.isTargetingAttack = false;
            Program.isTargetingBreach = false;
            DungeonManager.instance.mode = DungeonManager.Mode.Proc;
            DungeonManager.instance.activePlugin = procPlugins[procIndex];
        }
    }
    private void CancelTargeting()
    {
        Program.isTargetingAttack = false;
        Program.isTargetingBreach = false;
        Program.isTargetingProc = false;
        DungeonManager.instance.mode = DungeonManager.Mode.Move;
    }

    private void SwitchPanelVisibility(bool isVisible)
    {
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.enabled = isVisible;
        }
        foreach (Text text in GetComponentsInChildren<Text>())
        {
            text.enabled = isVisible;
        }
        foreach(Button button in GetComponentsInChildren<Button>())
        {
            button.enabled = isVisible;
        }
        hadTargetLastFrame = isVisible;
    }
}
