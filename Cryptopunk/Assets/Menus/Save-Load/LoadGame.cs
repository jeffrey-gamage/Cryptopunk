using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    [SerializeField] GameObject savedGamePanelPrefab;
    [SerializeField] GameObject persistentStatePrefab;
    [SerializeField] RectTransform[] saveFileAnchors;

    [SerializeField] GameObject scrollUpButton;
    [SerializeField] GameObject scrollDownButton;
    private int scrollIndex =0;
    // Start is called before the first frame update
    void Start()
    {
        if(PersistentState.instance)
        {
            PersistentState.instance.SaveProgress();
            Destroy(PersistentState.instance.gameObject);
        }
        RefreshSaves();
    }

    public void ScrollUp()
    {
        scrollIndex--;
        RefreshSaves();
    }

    public void ScrollDown()
    {
        scrollIndex++;
        RefreshSaves();
    }

    internal PersistentState InstantiateState()
    {
        return Instantiate(persistentStatePrefab).GetComponent<PersistentState>();
    }

    internal void BeginPlay()
    {
        SceneManager.LoadScene("desktop");
    }

    internal void RefreshSaves()
    {
        SavedGamePanel[] savedGamePanels = FindObjectsOfType<SavedGamePanel>();
        for(int i=0;i<savedGamePanels.Length;i++)
        {
            Destroy(savedGamePanels[i].gameObject);
        }
        string saveDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), PersistentState.saveGameDir);
        IEnumerable<string> saveFiles = Directory.EnumerateFiles(saveDirPath);
        List<string> saveFilesList = new List<string>();
        foreach(string saveFilePath in saveFiles)
        {
            saveFilesList.Add(saveFilePath);
        }
        for(int i=scrollIndex;i<saveFilesList.Count&&i<scrollIndex+saveFileAnchors.Length;i++)
        {
            SavedGamePanel savedGamePanel = Instantiate(savedGamePanelPrefab, saveFileAnchors[i - scrollIndex]).GetComponent<SavedGamePanel>();
            savedGamePanel.SetSavedGame(saveFilesList[i]);
        }
        ToggleScrollButton(CanScrollUp(), scrollUpButton);
        ToggleScrollButton(CanScrollDown(saveFilesList), scrollDownButton);
    }

    public void Back()
    {
        SceneManager.LoadScene("menu");
    }

    private bool CanScrollDown(List<string> saveFilesList)
    {
        return saveFilesList.Count - (scrollIndex + 1) > saveFileAnchors.Length;
    }
    private bool CanScrollUp()
    {
        return scrollIndex > 0;
    }

    private void ToggleScrollButton(bool canScroll, GameObject targetButton)
    {
        targetButton.GetComponent<Button>().enabled = canScroll;
        targetButton.GetComponent<Image>().enabled = canScroll;
        targetButton.GetComponentInChildren<Text>().enabled = canScroll;
    }
}
