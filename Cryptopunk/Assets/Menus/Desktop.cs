using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Desktop : MonoBehaviour
{
    public void Missions()
    {
        SceneManager.LoadScene("missions");
    }

    public void Shop()
    {
        SceneManager.LoadScene("shop");
    }
    public void Customize()
    {
        SceneManager.LoadScene("customize");
    }
    public void News()
    {
        SceneManager.LoadScene("news");
    }
    public void PowerOff()
    {
        SceneManager.LoadScene("menu");
    }
}
