using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramList : MonoBehaviour
{

    [SerializeField] Transform[] programAnchors;
    [SerializeField] GameObject listItem;
    [SerializeField] Button leftScroll;
    [SerializeField] Button rightScroll;
    private List<ProgramListItem> displayedPrograms;
    private PersistentState state;
    private int scrollIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        displayedPrograms = new List<ProgramListItem>();
        state = FindObjectOfType<PersistentState>();
        int i = scrollIndex;
        while(i<=scrollIndex+programAnchors.Length&&i<state.GetOwnedPrograms().Count)
        {
            ProgramListItem newListItem = Instantiate(listItem, programAnchors[i - scrollIndex].transform).GetComponent<ProgramListItem>();
            newListItem.SetProgram(state.GetOwnedPrograms()[i]);
            displayedPrograms.Add(newListItem);
            i++;
        }
        UpdateScrollEnabled();
    }

    public void ScrollLeft()
    {
        for(int i=0;i<programAnchors.Length-1;i++)
        {
            displayedPrograms[i].transform.position = programAnchors[i + 1].transform.position;
        }
        ProgramListItem temp = displayedPrograms[programAnchors.Length - 1];
        displayedPrograms.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex--;
        ProgramListItem newListItem = Instantiate(listItem, programAnchors[0].transform).GetComponent<ProgramListItem>();
        newListItem.SetProgram(state.GetOwnedPrograms()[scrollIndex]);
        displayedPrograms.Insert(0, newListItem);
        UpdateScrollEnabled();
    }

    public void ScrollRight()
    {
        for (int i = 1; i < programAnchors.Length; i++)
        {
            displayedPrograms[i].transform.position = programAnchors[i - 1].transform.position;
        }
        ProgramListItem temp = displayedPrograms[0];
        displayedPrograms.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex++;
        ProgramListItem newListItem = Instantiate(listItem, programAnchors[programAnchors.Length-1].transform).GetComponent<ProgramListItem>();
        newListItem.SetProgram(state.GetOwnedPrograms()[scrollIndex+programAnchors.Length-1]);
        displayedPrograms.Add(newListItem);
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
        return scrollIndex + programAnchors.Length < state.GetOwnedPrograms().Count;
    }
}
