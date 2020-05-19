using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleActivation();
        }
    }
    public void AbortMission()
    {
        Destroy(MissionStatus.instance.gameObject);
        SceneManager.LoadScene("desktop");
    }

    public void ToggleActivation()
    {
        bool newEnabledStatus = !GetComponent<Image>().enabled;
        foreach(Image image in GetComponentsInChildren<Image>())
        {
            image.enabled = newEnabledStatus;
        }
        foreach (Text text in GetComponentsInChildren<Text>())
        {
            text.enabled = newEnabledStatus;
        }
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.enabled = newEnabledStatus;
        }
    }

    internal bool isPaused()
    {
        return GetComponent<Image>().enabled;
    } 

    public void QuitGame()
    {
        Application.Quit();
    }
}
