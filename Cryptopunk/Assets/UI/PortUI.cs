using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortUI : MonoBehaviour
{
    private Port port;
    [SerializeField] GameObject programList;
    // Start is called before the first frame update

    public void DisplayProgramList()
    {
        Instantiate(programList.gameObject.transform);
    }

    public void Disengage()
    {
        //TODO: cash in all collected loot and leave the mission
    }
    
    private void DisplayUpdateList()
    {
        //TODO: allow player to select updates for ported in program
    }

    internal void Select(Program program)
    {
        port.Import(program);
    }

    internal void SetPort(Port port)
    {
        this.port = port;
    }
}
