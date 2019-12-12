using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortUI : MonoBehaviour
{
    private Port port;
    [SerializeField] GameObject programList;
    [SerializeField] Button disengageButton;
    [SerializeField] Button loadButton;
    // Start is called before the first frame update

    public void DisplayProgramList()
    {
        programList.SetActive(true);
        disengageButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
    }

    public void Disengage()
    {
        //TODO: cash in all collected loot and leave the mission
    }
    
    private void DisplayUpdateList()
    {
        //TODO: allow player to select updates for ported in program
    }

    internal void Select(GameObject program)
    {
        port.Import(program);
        Destroy(gameObject);
    }

    internal void SetPort(Port port)
    {
        this.port = port;
    }
}
