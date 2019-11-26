using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedIndicator : MonoBehaviour
{
    private SpriteRenderer selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        selectionIndicator = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Program.selectedProgram&&Program.selectedProgram.myTile)
        {
            selectionIndicator.enabled = true;
            gameObject.transform.position = Program.selectedProgram.transform.position;
        }
        else
        {
            selectionIndicator.enabled = false;
        }
    }
}
