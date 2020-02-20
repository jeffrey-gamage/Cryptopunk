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
    private List<ProgramMenuItem> displayedPrograms;
    private PersistentState state;
    private int scrollIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        displayedPrograms = new List<ProgramMenuItem>();
        state = FindObjectOfType<PersistentState>();
        RefreshProgramList();
    }

    private void RefreshProgramList()
    {
        foreach (ProgramMenuItem listItem in displayedPrograms)
        {
            Destroy(listItem.gameObject);
        }
        displayedPrograms.Clear();
        int i = scrollIndex;
        while (i <= scrollIndex + programAnchors.Length && i < state.GetOwnedPrograms().Count)
        {
            ProgramMenuItem newListItem = Instantiate(menuItem, programAnchors[i - scrollIndex].transform).GetComponent<ProgramMenuItem>();
            newListItem.SetProgram(state.GetOwnedPrograms()[i]);
            displayedPrograms.Add(newListItem);
            i++;
        }
        UpdateScrollEnabled();
    }

    public void ScrollLeft()
    {
        for (int i = 0; i < programAnchors.Length - 1; i++)
        {
            displayedPrograms[i].transform.position = programAnchors[i + 1].transform.position;
        }
        ProgramMenuItem temp = displayedPrograms[programAnchors.Length - 1];
        displayedPrograms.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex--;
        ProgramMenuItem newListItem = Instantiate(menuItem, programAnchors[0].transform).GetComponent<ProgramMenuItem>();
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
        ProgramMenuItem temp = displayedPrograms[0];
        displayedPrograms.Remove(temp);
        Destroy(temp.gameObject);
        scrollIndex++;
        ProgramMenuItem newListItem = Instantiate(menuItem, programAnchors[programAnchors.Length - 1].transform).GetComponent<ProgramMenuItem>();
        newListItem.SetProgram(state.GetOwnedPrograms()[scrollIndex + programAnchors.Length - 1]);
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
