using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Results : MonoBehaviour
{
    [SerializeField] string timeoutText = "connection timed out";
    [SerializeField] string wipeoutText = "all attack programs destroyed";
    [SerializeField] string disengageText = "attack package retrieved";
    [SerializeField] Text missionOutcome;
    [SerializeField] Text proceeds;
    // Start is called before the first frame update
    void Start()
    {
        DetermineMissionOutcome();
        DetermineMissionProceeds();
        Destroy(MissionStatus.instance.gameObject);
    }

    private void DetermineMissionProceeds()
    {
        if(MissionStatus.instance.hasMissionObjective)
        {
            proceeds.text = "mission successful\n";
            PersistentState.instance.progress++;
        }
        else
        {
            proceeds.text = "";
        }
        proceeds.text += "Attack proceeds:\n";
        PersistentState.instance.credits += MissionStatus.instance.lootValue;
        proceeds.text += "   " + MissionStatus.instance.lootValue.ToString() + " credits\n";
        if (MissionStatus.instance.outcome==MissionStatus.MissionOutcome.retrieved)
        {
            foreach(string schema in MissionStatus.instance.schematics)
            {
                PersistentState.instance.AddSchema(schema);
                proceeds.text += "program schematic: " + schema + "\n";
            }
        }
        else
        {
            proceeds.text = "You must retrieve one of your attack programs from a port in order to collect mission rewards.";
        }
    }

    private void DetermineMissionOutcome()
    {
        if (MissionStatus.instance.outcome == MissionStatus.MissionOutcome.retrieved)
        {
            missionOutcome.text = disengageText;
        }
        else if (MissionStatus.instance.outcome == MissionStatus.MissionOutcome.eliminated)
        {
            missionOutcome.text = wipeoutText;
        }
        else
        {
            missionOutcome.text = timeoutText;
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("desktop");
    }
}
