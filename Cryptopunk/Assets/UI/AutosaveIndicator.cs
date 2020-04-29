using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutosaveIndicator : MonoBehaviour
{
    internal void ToggleAutosaveIndicator(bool enable)
    {
        GetComponent<Image>().enabled = enable;
    }
}
