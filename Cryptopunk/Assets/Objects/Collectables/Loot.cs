using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public int credits = 50;
    public String schema;
    private static List<String> schemaNames;
    private bool isMissionObjective = false;
    private static bool isLibraryInitialized = false;
    // Start is called before the first frame update


    private static void PopulateLibrary()
    {
        if (!isLibraryInitialized)
        {
            schemaNames = new List<string>();
            foreach(GameObject schema in PersistentState.instance.schemaLibrary)
            {
                schemaNames.Add(schema.name);
            }
            isLibraryInitialized = true;
        }
    }

    internal void Yield()
    {
            AddCredits();
            AddSchema();
            Destroy(gameObject);
    }

    internal void setContents(int contents)
    {
        if(contents>0)
        {
            credits = contents;
        }
        else
        {
            schema = GetSchema(contents * -1);
        }
    }

    private static String GetSchema(int i)
    {
        PopulateLibrary();
        return schemaNames[i];
    }

    private void AddSchema()
    {
        if(schema.Length>0)
        {
            MissionStatus.instance.AddSchema(schema);
        }
    }

    private void AddCredits()
    {
        MissionStatus.instance.lootValue += credits;
    }
}
