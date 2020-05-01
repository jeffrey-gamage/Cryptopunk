using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OmniDataEntry : MonoBehaviour
{
    private string title;
    [SerializeField] Text label;

    internal void SetTitle(string newTitle)
    {
        title = newTitle;
        label.text = newTitle;
    }

    public void DisplayMyArticle()
    {
        FindObjectOfType<Omnipedia>().DisplayArticle(title);
    }
}
