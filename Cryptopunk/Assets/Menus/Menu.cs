using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play()
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
}
