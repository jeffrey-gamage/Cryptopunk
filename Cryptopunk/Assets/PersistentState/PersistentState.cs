using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentState : MonoBehaviour
{
    public static PersistentState instance;
    private List<GameObject> ownedPrograms;
    internal int credits;
    internal int progress;

    internal List<ExploitRecord> availableMissions;
    internal List<ShopInventoryRecord> shopInventorySchema;
    internal bool hasMissionListBeenRefreshed = false;
    internal bool hasInventoryBeenRefeshed = false;

    [SerializeField] string filename;
    [SerializeField] List<GameObject> startingPrograms;
    [SerializeField] int startingCredits = 250;
    [SerializeField] public GameObject[] schemaLibrary;

    public struct ExploitRecord
    {
        public int corpID;
        public int vulnerability;
        public string corpName;
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
            CreateStartingPackage();
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

    private void CreateStartingPackage()
    {
        credits = startingCredits;
        ownedPrograms = startingPrograms;
        progress = 0;
    }

    internal void AddProgram(GameObject newProgramSchema)
    {
        ownedPrograms.Add(newProgramSchema);
    }
    
    internal void AddSchema(string newSchema)
    {
        foreach(GameObject schema in schemaLibrary)
        {
            if(newSchema == schema.name)
            {
                ownedPrograms.Add(schema);
            }
        }
    }
}
