using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentComplete : MonoBehaviour
{
    [SerializeField] GameObject[] promptUIObjects;

    // Start is called before the first frame update
    void Start()
    {
        ToggleAll(false);
    }

    private void ToggleAll(bool isOn)
    {
        foreach (GameObject @object in promptUIObjects)
        {
            Image image = @object.GetComponent<Image>();
            Text text = @object.GetComponent<Text>();
            Button button = @object.GetComponent<Button>();
            if (image)
            {
                image.enabled = isOn;
            }
            if (text)
            {
                text.enabled = isOn;
            }
            if (button)
            {
                button.enabled = isOn;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDeploymentComplete())
        {
            ToggleAll(true);
        }
    }

    internal static bool IsDeploymentComplete()
    {
        bool isDeploymentComplete = true;
        foreach (Program program in DungeonManager.instance.GetPlayerControlledPrograms())
        {
            isDeploymentComplete &= program.myTile;
        }

        return isDeploymentComplete;
    }
}
