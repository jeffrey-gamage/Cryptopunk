using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramMenu : MonoBehaviour
{
    [SerializeField] Transform[] programAnchors;
    [SerializeField] GameObject menuItem;
    [SerializeField] Button leftScroll;
    [SerializeField] Button rightScroll;
    private List<MissionPrepMenuItem> displayedElements;
    private int scrollIndex = 0;
    private bool isDisplayingPlugins = false;
    // Start is called before the first frame update
    void Start()
    {
        displayedElements = new List<MissionPrepMenuItem>();
        RefreshProgramList();
    }

    public void RefreshProgramList()
    {
        isDisplayingPlugins = false;
        scrollIndex = 0;
        foreach (MissionPrepMenuItem listItem in displayedElements)
        {
            Destroy(listItem.gameObject);
        }
        displayedElements.Clear();
        int i = scrollIndex;
        while (i < scrollIndex + programAnchors.Length && i <PersistentState.instance.GetOwnedPrograms().Count)
        {
            MissionPrepMenuItem newListItem = Instantiate(menuItem, programAnchors[i - scrollIndex].transform).GetComponent<MissionPrepMenuItem>();
            newListItem.SetProgram(PersistentState.instance.GetOwnedPrograms()[i]);
            displayedElements.Add(newListItem);
            i++;
        }
        UpdateScrollEnabled();
    }
    public void RefreshPluginList()
    {
        isDisplayingPlugins = true;
        scrollIndex = 0;
        foreach (MissionPrepMenuItem listItem in displayedElements)
        {
            Destroy(listItem.gameObject);
        }
        displayedElements.Clear();
        int i = scrollIndex;
        if (PersistentState.instance.GetOwnedPlugins().Count > 0)
        {
            while (i < scrollIndex + programAnchors.Length && i < PersistentState.instance.GetOwnedPlugins().Count)
            {
                MissionPrepMenuItem newListItem = Instantiate(menuItem, programAnchors[i - scrollIndex].transform).GetComponent<MissionPrepMenuItem>();
                newListItem.SetPlugin(PersistentState.instance.GetOwnedPlugins()[i]);
                displayedElements.Add(newListItem);
                i++;
            }
        }
        UpdateScrollEnabled();
    }

    public void ScrollLeft()
    {
        for (int i = 0; i < programAnchors.Length - 1; i++)
        {
            displayedElements[i].transform.position = programAnchors[i + 1].transform.position;
        }
        MissionPrepMenuItem temp = displayedElements[programAnchors.Length - 1];
        displayedElements.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex--;
        MissionPrepMenuItem newListItem = Instantiate(menuItem, programAnchors[0].transform).GetComponent<MissionPrepMenuItem>();
        newListItem.SetProgram(PersistentState.instance.GetOwnedPrograms()[scrollIndex]);
        displayedElements.Insert(0, newListItem);
        UpdateScrollEnabled();
    }

    public void ScrollRight()
    {
        for (int i = 1; i < programAnchors.Length; i++)
        {
            displayedElements[i].transform.position = programAnchors[i - 1].transform.position;
        }
        MissionPrepMenuItem temp = displayedElements[0];
        displayedElements.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex++;
        MissionPrepMenuItem newListItem = Instantiate(menuItem, programAnchors[programAnchors.Length - 1].transform).GetComponent<MissionPrepMenuItem>();
        newListItem.SetProgram(PersistentState.instance.GetOwnedPrograms()[scrollIndex + programAnchors.Length - 1]);
        displayedElements.Add(newListItem);
        UpdateScrollEnabled();
    }
    private void UpdateScrollEnabled()
    {
        leftScroll.enabled = CanScrollLeft();
        rightScroll.enabled = CanScrollRight();
    }
    private bool CanScrollLeft()
    {
        return scrollIndex > 0;
    }
    private bool CanScrollRight()
    {
        if(isDisplayingPlugins)
        {
            return scrollIndex + programAnchors.Length < PersistentState.instance.GetOwnedPlugins().Count;
        }
        return scrollIndex + programAnchors.Length < PersistentState.instance.GetOwnedPrograms().Count;
    }
}
