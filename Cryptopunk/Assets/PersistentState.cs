using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentState : MonoBehaviour
{
    private List<GameObject> ownedPrograms;
    internal int credits;
    internal int progress;
    [SerializeField] string filename;
    [SerializeField] List<GameObject> startingPrograms;
    [SerializeField] int startingCredits = 250;
    [SerializeField] public GameObject[] schemaLibrary;
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
            DontDestroyOnLoad(gameObject);
            CreateStartingPackage();
        }
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

    void Update()
    {
        
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
