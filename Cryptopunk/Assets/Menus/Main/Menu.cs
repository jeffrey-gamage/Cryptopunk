using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("newGame");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("loadGame");
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
}
