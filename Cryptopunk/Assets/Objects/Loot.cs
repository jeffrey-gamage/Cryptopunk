using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public int credits = 50;
    public String schema;
    private static List<String> schemaLibrary;
    private bool isMissionObjective = false;
    private static bool isLibraryInitialized = false;
    // Start is called before the first frame update


    private static void PopulateLibrary()
    {
        if (!isLibraryInitialized)
        {
            schemaLibrary = new List<string>();
            schemaLibrary.Add("Harrier");
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
        return schemaLibrary[i];
    }

    private void AddSchema()
    {
        if(schema.Length>0)
        {
                FindObjectOfType<PersistentState>().AddSchema(schema);
        }
    }

    private void AddCredits()
    {
        FindObjectOfType<PersistentState>().credits += credits;
    }
}
