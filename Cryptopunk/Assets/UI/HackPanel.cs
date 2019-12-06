using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackPanel : MonoBehaviour
{
    [SerializeField] Text toggleButtonText;
    [SerializeField] Text objectName;
    [SerializeField] Text integrity;
    [SerializeField] Text rebootCounter;
    [SerializeField] Text status;
    [SerializeField] Image toggleButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hackable.selectedObject)
        {
            objectName.text = Hackable.selectedObject.gameObject.name;
            integrity.text = Hackable.selectedObject.currentIntegrity.ToString() + " / " + Hackable.selectedObject.maxIntegrity.ToString();
            if(Hackable.selectedObject.IsHacked())
            {
                rebootCounter.text = Hackable.selectedObject.rebootCountdown.ToString() + " turns until reboot";
            }
            else
            {
                rebootCounter.text = "status: secure";
            }
            if(!Hackable.selectedObject.myProgram&&Hackable.selectedObject.IsHacked())
            {
                toggleButton.enabled = true;
                if(Hackable.selectedObject.isEnabled)
                {
                    status.text = "enabled";
                    toggleButtonText.text = "disable";
                }
                else
                {
                    status.text = "disabled";
                    toggleButtonText.text = "enable";
                }
            }
            else
            {
                status.text = "";
                toggleButton.enabled = false;
                toggleButtonText.text = "";
            }
        }
    }

    void Toggle()
    {
        if(Hackable.selectedObject)
        {
            Hackable.selectedObject.isEnabled = !Hackable.selectedObject.isEnabled;
        }
    }
}
