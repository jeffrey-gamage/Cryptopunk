using System;
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

    bool hadTargetLastFrame = true;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(Hackable.selectedObject)
        {
            if(!hadTargetLastFrame)
            {
                SwitchPanelVisibility(true);
            }
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
        else
        {
            if(hadTargetLastFrame)
            {
                SwitchPanelVisibility(false);
            }
        }
    }

    private void SwitchPanelVisibility(bool isVisible)
    {
        foreach(Image image in GetComponentsInChildren<Image>())
        {
            image.enabled = isVisible;
        }
        foreach(Text text in GetComponentsInChildren<Text>())
        {
            text.enabled = isVisible;
        }
        toggleButton.enabled = isVisible;
        hadTargetLastFrame = isVisible;
    }

    public void Toggle()
    {
        if(Hackable.selectedObject)
        {
            if(Hackable.selectedObject.isEnabled)
            {
                Hackable.selectedObject.Deactivate(false);
            }
            else
            {
                Hackable.selectedObject.Activate();
            }
        }
    }
}
