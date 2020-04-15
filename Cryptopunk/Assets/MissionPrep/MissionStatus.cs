using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MissionStatus : MonoBehaviour
{
    internal enum MissionOutcome  {
        eliminated,
        timeout,
        retrieved
    }
    internal MissionOutcome outcome = MissionOutcome.timeout;
    //mission results tracker
    internal int lootValue = 0;
    internal bool hasMissionObjective = false;
    internal List<String> schematics = new List<string>();

    internal static MissionStatus instance;
    [SerializeField] List<Image> programSlots;
    internal List<GameObject>[] pluginSlots;

    [SerializeField] GameObject pluginSlotPrefab;
    [SerializeField] Vector3 firstPluginSlotOffset;
    [SerializeField] Vector3 subsequentPluginSlotOffset;
    internal int kbBudget;
    private int totalBudget;
    internal int selectedSlotIndex = 0;
    public Image selectedSlotHighlight;
    internal string corpName;
    internal int corpID;
    internal GameObject[] selectedPrograms;
    internal List<GameObject>[] selectedPlugins;
    [SerializeField] Text targetCorpName;
    [SerializeField] Text budgetDisplay;
    [SerializeField] Text budgetAvailableDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Exploit targetExploit = FindObjectOfType<Exploit>();
            if (targetExploit)
            {
                kbBudget = targetExploit.vulnerability;
                totalBudget = kbBudget;
                corpName = targetExploit.corpName;
                corpID = targetExploit.corpID;
                Destroy(targetExploit.gameObject);
                selectedPrograms = new GameObject[programSlots.Count];
                selectedPlugins = new List<GameObject>[programSlots.Count];
                pluginSlots = new List<GameObject>[programSlots.Count];
                for(int i=0;i<programSlots.Count;i++)
                {
                    pluginSlots[i] = new List<GameObject>();
                    selectedPlugins[i] = new List<GameObject>();
                }
                targetCorpName.text = "target: " + corpName;
                budgetDisplay.text = "maximum package size: " + totalBudget + " kb";
            }
        }
    }

    internal void ResetPluginIcon(int programToRemoveFromIndex,string pluginName)
    {
        if (pluginSlots[programToRemoveFromIndex].Count > 1)
        {
            bool hasFoundPluginToRemove = false;
            for (int i = 0; i < pluginSlots[programToRemoveFromIndex].Count; i++)
            {
                if(i<selectedPlugins[programToRemoveFromIndex].Count&&selectedPlugins[programToRemoveFromIndex][i].name==pluginName)
                {
                    hasFoundPluginToRemove = true;
                }
                else if(hasFoundPluginToRemove)
                {
                    pluginSlots[programToRemoveFromIndex][i-1].GetComponent<PluginSlot>().SetPlugin(pluginSlots[programToRemoveFromIndex][i].GetComponent<PluginSlot>().selectedPlugin);
                }
            }
        }
    }

    internal void SetPlugin(int programRecievingPluginIndex, Plugin plugin)
    {
        for (int i = 0; i < pluginSlots[programRecievingPluginIndex].Count; i++)
        {
            if (pluginSlots[programRecievingPluginIndex][i].GetComponent<PluginSlot>().IsEmpty())
            {
                pluginSlots[programRecievingPluginIndex][i].GetComponent<PluginSlot>().SetPlugin(plugin);
                break;
            }
        }
    }

    internal void AddSchema(string schema)
    {
        schematics.Add(schema);
    }


    internal void CollectMissionObjective()
    {
        hasMissionObjective = true;
    }

    private void Update()
    {
        if (budgetAvailableDisplay)
        {
            budgetAvailableDisplay.text = "available space: " + kbBudget + " kb";
        }
    }

    internal int GetTotalBudget()
    {
        return totalBudget;
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

    internal void AttemptInstallPlugin(GameObject myPrefab)
    {
        myPrefab.GetComponent<Plugin>().AttemptAddToPackage(selectedSlotIndex);
    }

    private bool SubmissionIsInBudget(GameObject selectedProgram)
    {
        int costIncrease = 0;
        if (selectedPrograms[selectedSlotIndex])
        {
            costIncrease = selectedProgram.GetComponent<Program>().GetSize() - selectedPrograms[selectedSlotIndex].GetComponent<Program>().GetSize();
        }
        else
        {
            costIncrease = selectedProgram.GetComponent<Program>().GetSize();
        }
        return kbBudget >= costIncrease;
    }

    public void BackToMissionSelect()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("missions");
    }

    public void LaunchMission()
    {
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
        FindObjectOfType<DungeonDirectory>().LoadCorpDungeon(corpID);
    }

    public void SelectSlot(int slotNum)
    {
        selectedSlotIndex = slotNum;
        selectedSlotHighlight.gameObject.transform.position = programSlots[slotNum].transform.position;
    }

    private void AddProgramToPackage(GameObject selectedProgram)
    {
        selectedPrograms[selectedSlotIndex] = selectedProgram;
        kbBudget -= selectedProgram.GetComponent<Program>().GetSize();
        programSlots[selectedSlotIndex].sprite = selectedProgram.GetComponentInChildren<SpriteRenderer>().sprite;
        CreatePluginSlots();
    }

    private void CreatePluginSlots()
    {
        for(int i=0;i<selectedPrograms[selectedSlotIndex].GetComponent<PlayerProgram>().pluginSlots;i++)
        {
            PluginSlot newSlot = Instantiate(pluginSlotPrefab, programSlots[selectedSlotIndex].transform).GetComponent<PluginSlot>();
            newSlot.transform.position += firstPluginSlotOffset + subsequentPluginSlotOffset * i;
            newSlot.myProgramSlotIndex = selectedSlotIndex;
            pluginSlots[selectedSlotIndex].Add(newSlot.gameObject);
        }
    }

    private void RemoveProgramFromPackage(GameObject programToRemove)
    {
        selectedPrograms[selectedSlotIndex] = null;
        if (pluginSlots[selectedSlotIndex].Count > 0)
        {
            for (int i = 0; i < pluginSlots[selectedSlotIndex].Count; i++)
            {
                Destroy(pluginSlots[selectedSlotIndex][i]);
            }
            pluginSlots[selectedSlotIndex].Clear();
            foreach(GameObject plugin in selectedPlugins[selectedSlotIndex])
            {
                plugin.GetComponent<Plugin>().RemoveFromPackage(selectedSlotIndex);
            }
            selectedPlugins[selectedSlotIndex].Clear();
        }
        kbBudget += programToRemove.GetComponent<Program>().GetSize();
    }
}
