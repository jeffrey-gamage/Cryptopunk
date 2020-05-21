using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortUI : MonoBehaviour
{
    private Port port;
    [SerializeField] GameObject programList;
    [SerializeField] Button disengageButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button cancelButton;
    // Start is called before the first frame update

    public void DisplayProgramList()
    {
        programList.SetActive(true);
        disengageButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    public void HideProgramList()
    {
        programList.SetActive(false);
        disengageButton.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    public void Cancel()
    {
        port.isEnabled = false;
        Destroy(gameObject);
    }

    public void Disengage()
    {
        MissionStatus.instance.outcome = MissionStatus.MissionOutcome.retrieved;
        SceneManager.LoadScene("results");
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
