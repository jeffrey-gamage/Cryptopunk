using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectionIndicator : MonoBehaviour
{
    private Vector3 menuOffset;


    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = MenuButton.selectedOption.transform.position + menuOffset;
    }

    internal void Initialize()
    {
        menuOffset = gameObject.transform.position - MenuButton.selectedOption.transform.position;
    }
}
