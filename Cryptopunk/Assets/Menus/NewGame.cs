using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    private bool isPlayingTutorial = true;
    [SerializeField] GameObject persistentStatePrefab;
    [SerializeField] Text tutorialButtonText;
    [SerializeField] InputField fileName;

    private void Start()
    {

    }

    public void ToggleTutorial()
    {
        isPlayingTutorial = !isPlayingTutorial;
        if(isPlayingTutorial)
        {
            tutorialButtonText.text = "X";
        }
        else
        {
            tutorialButtonText.text = "";
        }
    }

    public void StartGame()
    {
        if (PersistentState.instance)
        {
            PersistentState.instance.SaveProgress();
            Destroy(PersistentState.instance.gameObject);
        }
        PersistentState newState =Instantiate(persistentStatePrefab).GetComponent<PersistentState>();
        newState.SetFileName(fileName.text);
        if (isPlayingTutorial)
        {
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            SceneManager.LoadScene("Desktop");
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
