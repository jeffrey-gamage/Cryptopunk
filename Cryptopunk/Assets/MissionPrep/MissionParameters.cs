using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MissionParameters : MonoBehaviour
{
    [SerializeField] List<Image> programSlots;
    internal int kbBudget;
    internal int selectedSlotIndex = 0;
    public Image selectedSlotHighlight;
    internal string corpName;
    internal GameObject[] selectedPrograms;

    [SerializeField] Text targetCorpName;
    [SerializeField] Text budgetDisplay;
    [SerializeField] Text budgetAvailableDisplay;
    // Start is called before the first frame update
    void Start()
    {
        Exploit targetExploit = FindObjectOfType<Exploit>();
        kbBudget = targetExploit.vulnerability;
        corpName = targetExploit.corpName;
        Destroy(targetExploit.gameObject);
        selectedPrograms = new GameObject[programSlots.Count];

        targetCorpName.text = "target: " + corpName;
        budgetDisplay.text = "maximum package size: " + kbBudget;
    }

    private void Update()
    {
        budgetAvailableDisplay.text = "available space: " + kbBudget;
    }

    public void SubmitProgramToPackage(GameObject selectedProgram)
    {
        if (SubmissionIsInBudget(selectedProgram))
        {
            if (selectedPrograms[selectedSlotIndex])
            {
                RemoveProgramFromPackage(selectedPrograms[selectedSlotIndex]);
            }
            AddProgramToPackage(selectedProgram);
            SelectSlot((selectedSlotIndex + 1) % programSlots.Count);
        }
        else
        {
            Debug.Log("cannot add program without going over budget");
            //TODO: add budget feedback player can see
        }
    }

    private bool SubmissionIsInBudget(GameObject selectedProgram)
    {
        int costIncrease = 0;
        if (selectedPrograms[selectedSlotIndex])
        {
            costIncrease = selectedProgram.GetComponent<Program>().maxSize - selectedPrograms[selectedSlotIndex].GetComponent<Program>().maxSize;
        }
        else
        {
            costIncrease = selectedProgram.GetComponent<Program>().maxSize;
        }
        return kbBudget >= costIncrease;
    }

    public void BackToMissionSelect()
    {
        SceneManager.LoadScene("missions");
    }

    public void LaunchMission()
    {
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("procDungeon");
    }

    public void SelectSlot(int slotNum)
    {
        selectedSlotIndex = slotNum;
        selectedSlotHighlight.gameObject.transform.position = programSlots[slotNum].transform.position;
    }

    private void AddProgramToPackage(GameObject selectedProgram)
    {
        selectedPrograms[selectedSlotIndex] = selectedProgram;
        kbBudget -= selectedProgram.GetComponent<Program>().maxSize;
        programSlots[selectedSlotIndex].sprite = selectedProgram.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void RemoveProgramFromPackage(GameObject programToRemove)
    {
        selectedPrograms[selectedSlotIndex] = null;
        kbBudget += programToRemove.GetComponent<Program>().maxSize;
    }
}
