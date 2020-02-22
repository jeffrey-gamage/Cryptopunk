using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour
{
    [SerializeField] string timeoutText = "connection timed out";
    [SerializeField] string wipeoutText = "all attack programs destroyed";
    [SerializeField] string disengageText = "attack package retrieved";
    [SerializeField] Text missionOutcome;
    [SerializeField] Text proceeds;
    // Start is called before the first frame update
    void Start()
    {
        if(MissionStatus.instance.outcome==MissionStatus.MissionOutcome.retrieved)
        {
            missionOutcome.text = disengageText;
        }
        else if(MissionStatus.instance.outcome==MissionStatus.MissionOutcome.eliminated)
        {
            missionOutcome.text = wipeoutText;
        }
        else
        {
            missionOutcome.text = timeoutText;
        }
        Destroy(MissionStatus.instance.gameObject);
    }
    

    public void Exit()
    {
        SceneManager.LoadScene("desktop");
    }
}
