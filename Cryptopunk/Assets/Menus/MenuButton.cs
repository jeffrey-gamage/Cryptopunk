using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    internal static MenuButton selectedOption;

    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip mouseOverSound;
    [SerializeField] bool isInitialSelection;

    private void Start()
    {
        if(isInitialSelection)
        {
            selectedOption = this;
            FindObjectOfType<MenuSelectionIndicator>().Initialize();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSource.PlayClipAtPoint(selectSound, Vector3.zero,PlayerPrefs.GetFloat(Options.sfxVolumeKey));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this != selectedOption)
        {
            selectedOption = this;
            AudioSource.PlayClipAtPoint(mouseOverSound, Vector3.zero, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
        }
    }
}
