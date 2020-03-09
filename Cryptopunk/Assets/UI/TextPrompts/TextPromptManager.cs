using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPromptManager : MonoBehaviour
{
    [SerializeField] GameObject[] textPromptObjects;
    private List<TextPrompt> textPrompts;
    // Start is called before the first frame update
    void Start()
    {
        textPrompts = new List<TextPrompt>();
        foreach(GameObject @object in textPromptObjects)
        {
            textPrompts.Add(@object.GetComponent<TextPrompt>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        TextPrompt toDisplay = null;
        if (DeploymentComplete.IsDeploymentComplete())
        {
            foreach (Program program in DungeonManager.instance.GetPlayerControlledPrograms())
            {
                foreach (TextPrompt prompt in textPrompts)
                {
                    foreach (Vector3Int triggerPoint in prompt.TriggerPoints)
                    {
                        if (triggerPoint.x == program.myTile.xCoord && triggerPoint.z == program.myTile.zCoord)
                        {
                            toDisplay = prompt;
                        }
                    }
                }
            }
        }
        if(toDisplay)
        {
            Instantiate(toDisplay, FindObjectOfType<Canvas>().transform);
            textPrompts.Remove(toDisplay);
        }
    }
}
