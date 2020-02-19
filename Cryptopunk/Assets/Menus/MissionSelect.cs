using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionSelect : MonoBehaviour
{
    [SerializeField] RectTransform[] anchors;
    [SerializeField] GameObject exploitPrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach(RectTransform anchor in anchors)
        {
            Instantiate(exploitPrefab, anchor);
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("Desktop");
    }

}
