using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private bool isShowingTutorialSelect = false;
    [SerializeField] Button tutorialSelectButton;
    [SerializeField] Button skipTutorialButton;
    public void ShowTutorialSelect()
    {
        isShowingTutorialSelect = !isShowingTutorialSelect;
        tutorialSelectButton.enabled = isShowingTutorialSelect;
        tutorialSelectButton.GetComponent<Text>().enabled = isShowingTutorialSelect;
        skipTutorialButton.enabled = isShowingTutorialSelect;
        skipTutorialButton.GetComponent<Text>().enabled = isShowingTutorialSelect;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("desktop");
    }
    public void Options()
    {
        SceneManager.LoadScene("options");
    }
    public void Credits()
    {
        SceneManager.LoadScene("credits");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void PlayTutorial()
    {
        SceneManager.LoadScene("tutorial");
    }
}
