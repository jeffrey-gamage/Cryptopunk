using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentState : MonoBehaviour
{
    public static PersistentState instance;
    private List<GameObject> ownedPrograms;
    private List<GameObject> ownedPlugins;
    internal int credits;
    internal int progress;

    internal List<ExploitRecord> availableMissions;
    internal List<ShopInventoryRecord> shopInventorySchema;
    internal bool hasMissionListBeenRefreshed = false;
    internal bool hasInventoryBeenRefeshed = false;
    protected bool isNewGame = true;
    internal List<int> usedFinalRoomIndices;

    [SerializeField] string filename;
    internal static string saveGameDir = "\\Saved_Games\\Cryptopunk";
    [SerializeField] List<GameObject> startingPrograms;
    [SerializeField] int startingCredits = 250;
    [SerializeField] public GameObject[] schemaLibrary;

    public struct ExploitRecord
    {
        public int corpID;
        public int vulnerability;
        public string corpName;
    }

    internal void SetFileName(string newName)
    {
        filename = newName;
    }

    public struct ShopInventoryRecord
    {
        public string schemaName;
        public int cost;
    }
    // Start is called before the first frame update
    void Start()
    {
        //singleton
        if(FindObjectsOfType<PersistentState>().Length>1)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (isNewGame)
            {
                CreateStartingPackage();
                SaveProgress();
            }
        }
    }

    internal GameObject GetProgramPrefab(string schemaName)
    {
        foreach(GameObject programSchema in schemaLibrary)
        {
            if(programSchema.name==schemaName)
            {
                return programSchema;
            }
        }
        throw new Exception("schema name not recognized");
    }

    public List<GameObject> GetOwnedPrograms()
    {
        return ownedPrograms;
    }
    public List<GameObject> GetOwnedPlugins()
    {
        return ownedPlugins;
    }

    private void CreateStartingPackage()
    {
        credits = startingCredits;
        ownedPrograms = startingPrograms;
        ownedPlugins = new List<GameObject>();
        availableMissions = new List<ExploitRecord>();
        shopInventorySchema = new List<ShopInventoryRecord>();
        usedFinalRoomIndices = new List<int>();
        progress = 0;
    }

    internal void AddProgram(GameObject newProgramSchema)
    {
        ownedPrograms.Add(newProgramSchema);
    }
    internal void AddPlugin(GameObject newPluginSchema)
    {
        ownedPlugins.Add(newPluginSchema);
    }
    
    internal void AddSchema(string newSchema)
    {
        foreach(GameObject schema in schemaLibrary)
        {
            if(newSchema == schema.name)
            {
                if (schema.GetComponent<PlayerProgram>())
                {
                    ownedPrograms.Add(schema);
                }
                else
                {
                    ownedPlugins.Add(schema);
                }
            }
        }
    }

    public void SaveProgress()
    {
        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), saveGameDir);
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, filename);
        StreamWriter saveFile = new StreamWriter(filePath);
        WriteOwnedObjects(ref saveFile);
        WriteShopInventory(ref saveFile);
        WriteAvailableMissions(ref saveFile);
        WriteRefreshStatus(ref saveFile);
        WriteCredits(ref saveFile);
        WriteProgress(ref saveFile);
        WriteUsedFinalRooms(ref saveFile);
        WriteEndfile(ref saveFile);
        saveFile.Close();
    }

    private void WriteRefreshStatus(ref StreamWriter saveFile)//Is a fixed length entry, does not require an ending marker
    {
        saveFile.WriteLine("++REFRESH STATUS++");
        saveFile.WriteLine(hasInventoryBeenRefeshed.ToString());
        saveFile.WriteLine(hasMissionListBeenRefreshed.ToString());
    }

    private void WriteEndfile(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++ENDFILE++");
        Debug.Log("++ENDFILE");
    }

    private void WriteProgress(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++PROGRESS++");
        saveFile.WriteLine(progress.ToString());
    }

    private void WriteCredits(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++CREDITS++");
        Debug.Log("++CREDITS++");
        saveFile.WriteLine(credits.ToString());
        Debug.Log(credits.ToString());
    }

    private void WriteAvailableMissions(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++MISSIONS++");
        foreach(ExploitRecord record in availableMissions)
        {
            saveFile.WriteLine(record.corpID.ToString() + "-" + record.vulnerability.ToString() + "-" + record.corpName);
        }
        saveFile.WriteLine("++END++");
    }

    private void WriteShopInventory(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++SHOP INVENTORY++");
        foreach (ShopInventoryRecord record in shopInventorySchema)
        {
            saveFile.WriteLine(record.schemaName+"-"+record.cost.ToString());
        }
        saveFile.WriteLine("++END++");
    }

    private void WriteUsedFinalRooms(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++USED FINAL ROOMS++");
        foreach(int roomIndex in usedFinalRoomIndices)
        {
            saveFile.WriteLine(roomIndex.ToString());
        }
        saveFile.WriteLine("++END++");
    }

    private void WriteOwnedObjects(ref StreamWriter saveFile)
    {
        saveFile.WriteLine("++OWNED OBJECTS++");
        foreach (GameObject program in ownedPrograms)
        {
            saveFile.WriteLine(program.name);
        }
        foreach (GameObject plugin in ownedPlugins)
        {
            saveFile.WriteLine(plugin.name);
        }
        saveFile.WriteLine("++END++");
    }

    internal void LoadProgress(string fileName)
    {
        ownedPrograms = new List<GameObject>();
        ownedPlugins = new List<GameObject>();
        usedFinalRoomIndices = new List<int>();
        availableMissions = new List<ExploitRecord>();
        shopInventorySchema = new List<ShopInventoryRecord>();
        isNewGame = false;
        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), saveGameDir);
        string filePath = Path.Combine(savePath, fileName);
        try
        {   // Open the text file using a stream reader.
            using (StreamReader saveFile = new StreamReader(filePath))
            {
                // Read the stream to a string, and write the string to the console.
                String fileText = saveFile.ReadToEnd();
                fileText = ReadFromSaveFile(ref fileText);
                filename = fileName;
                saveFile.Close();
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    private string ReadFromSaveFile(ref string fileText)
    {
        string nextLine = GetNextLine(ref fileText);
        switch (nextLine)
        {
            case "++OWNED OBJECTS++":
                {
                    ParseOwnedObjects(ref fileText);
                    break;
                }
            case "++SHOP INVENTORY++":
                {
                    ParseShopInventory(ref fileText);
                    break;
                }
            case "++MISSIONS++":
                {
                    ParseAvailableMissions(ref fileText);
                    break;
                }
            case "++REFRESH STATUS++":
                {
                    ParseRefreshStatus(ref fileText);
                    break;
                }
            case "++CREDITS++"://Is a single line entry, does not require an ending marker
                {
                    ParseCredits(ref fileText);
                    break;
                }
            case "++USED FINAL ROOMS++":
                {
                    ParseFinalRoomsUsed(ref fileText);
                    break;
                }
            case "++PROGRESS++"://Is a single line entry, does not require an ending marker
                {
                    ParseProgress(ref fileText);
                    break;
                }
            case "++ENDFILE++":
                {
                    break;
                }
        }
        return fileText;
    }

    private void ParseRefreshStatus(ref string fileText)
    {
        if(GetNextLine(ref fileText)=="False")
        {
            hasInventoryBeenRefeshed = false;
        }
        else
        {
            hasInventoryBeenRefeshed = true;
        }
        if (GetNextLine(ref fileText) == "False")
        {
            hasMissionListBeenRefreshed = false;
        }
        else
        {
            hasMissionListBeenRefreshed = true;
        }
        ReadFromSaveFile(ref fileText);
    }

    private static string GetNextLine(ref string saveText)
    {
        int indexOfNextNewline = saveText.IndexOf("\n");
        string nextLine = saveText.Substring(0, indexOfNextNewline - 1);
        saveText = saveText.Substring(indexOfNextNewline + 1);
        return nextLine;
    }

    protected void ParseOwnedObjects(ref string fileText)
    {
        string nextLine = GetNextLine(ref fileText);
        while (nextLine != "++END++")
        {
            AddSchema(nextLine);
            nextLine = GetNextLine(ref fileText);
        }
        ReadFromSaveFile(ref fileText);
    }

    protected void ParseShopInventory(ref string fileText)
    {
        shopInventorySchema = new List<ShopInventoryRecord>();
        string nextLine = GetNextLine(ref fileText);
        while (nextLine != "++END++")
        {
            string itemName = "";
            int cost = 0;
            bool isCost = false;
            foreach(char nextChar in nextLine)
            {
                if (nextChar == '-')
                {
                    isCost = true;
                }
                else if (isCost && Char.IsDigit(nextChar))
                {
                    cost = cost * 10 + int.Parse(nextChar.ToString());
                }
                else itemName += nextChar;
            }
            ShopInventoryRecord newInventoryRecord;
            newInventoryRecord.cost = cost;
            newInventoryRecord.schemaName = itemName;
            shopInventorySchema.Add(newInventoryRecord);
            nextLine = GetNextLine(ref fileText);
        }
        hasInventoryBeenRefeshed = true;
        ReadFromSaveFile(ref fileText);
    }

    protected void ParseAvailableMissions(ref string fileText)
    {
        availableMissions = new List<ExploitRecord>();
        string nextLine = GetNextLine(ref fileText);
        while (nextLine != "++END++")
        {
            string corpName = "";
            int corpID = 0;
            int vulnerability = 0;
            bool isVunerabilityRecorded = false;
            bool isCorpIDRecorded = false;
            foreach (char nextChar in nextLine)
            {
                if (nextChar == '-')
                {
                    if (!isCorpIDRecorded)
                    {
                        isCorpIDRecorded = true;
                    }
                    else
                    {
                        isVunerabilityRecorded = true;
                    }
                }
                else if (Char.IsDigit(nextChar))
                {
                    if (!isCorpIDRecorded)
                    {
                        corpID = corpID * 10 + int.Parse(nextChar.ToString());
                    }
                    else if(!isVunerabilityRecorded)
                    {
                        vulnerability = vulnerability * 10 + int.Parse(nextChar.ToString());
                    }
                }
                else corpName += nextChar;
            }
            ExploitRecord newExploitRecord;
            newExploitRecord.corpID = corpID;
            newExploitRecord.corpName = corpName;
            newExploitRecord.vulnerability = vulnerability;
            availableMissions.Add(newExploitRecord);
            nextLine = GetNextLine(ref fileText);
        }
        hasMissionListBeenRefreshed = true;
        ReadFromSaveFile(ref fileText);
    }

    protected void ParseCredits(ref string fileText)//Is a single line entry, does not require an ending marker
    {
        string nextLine = GetNextLine(ref fileText);
        int credits = 0;
        foreach (char nextChar in nextLine)
        {
            if (char.IsDigit(nextChar))
            {
                credits = credits * 10 + int.Parse(nextChar.ToString());
            }
        }
        this.credits = credits;
        ReadFromSaveFile(ref fileText);
    }

    protected void ParseFinalRoomsUsed(ref string fileText)
    {
        string nextLine = GetNextLine(ref fileText);
        while(nextLine!="++END++")
        {
            int roomIndex = 0;
            foreach (char nextChar in nextLine)
            {
                if (char.IsDigit(nextChar))
                {
                    roomIndex = roomIndex * 10 + int.Parse(nextChar.ToString());
                }
            }
            usedFinalRoomIndices.Add(roomIndex);
            nextLine = GetNextLine(ref fileText);
        }
    }
    protected void ParseProgress(ref string fileText)//Is a single line entry, does not require an ending marker
    {
        string nextLine = GetNextLine(ref fileText);
        int progress = 0;
        foreach (char nextChar in nextLine)
        {
            if (char.IsDigit(nextChar))
            {
                progress = progress * 10 + int.Parse(nextChar.ToString());
            }
        }
        this.progress = progress;
        ReadFromSaveFile(ref fileText);
    }
}
