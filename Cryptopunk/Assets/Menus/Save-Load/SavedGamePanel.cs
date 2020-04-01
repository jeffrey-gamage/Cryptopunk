using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavedGamePanel : MonoBehaviour
{
    [SerializeField] Text savedGameLabel;
    private LoadGame loadGame;
    private string savedGamePath;

    private void Start()
    {
        loadGame = FindObjectOfType<LoadGame>();
    }

    public void LoadGame()
    {
        PersistentState newState = loadGame.InstantiateState();
        newState.LoadProgress(savedGamePath);
        loadGame.BeginPlay();
    }

    public void DeleteSave()
    {
        File.Delete(savedGamePath);
        Destroy(gameObject);
        loadGame.RefreshSaves();
    }

    internal void SetSavedGame(string savedGamePath)
    {
        this.savedGamePath = savedGamePath;
        savedGameLabel.text = savedGamePath.Substring(savedGamePath.LastIndexOf('\\'),savedGamePath.LastIndexOf('.')- savedGamePath.LastIndexOf('\\'));
    }

}
