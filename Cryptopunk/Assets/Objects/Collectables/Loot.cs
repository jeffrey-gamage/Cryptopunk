using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] GameObject pickupMessage;
    [SerializeField] AudioClip pickupClip;
    [SerializeField] GameObject body;
    private static float spinSpeed = 30f;
    public int credits = 50;
    public String schema;
    private static List<String> schemaNames;
    [SerializeField] bool isMissionObjective = false;
    private static bool isLibraryInitialized = false;

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
    private void Update()
    {
        gameObject.transform.Rotate(0,spinSpeed * Time.deltaTime,0);
    }

    internal void Yield()
    {
            AddCredits();
            AddSchema();
            if(isMissionObjective)
            {
                MissionStatus.instance.CollectMissionObjective();
            }
        LootMessage message = Instantiate(pickupMessage, gameObject.transform.position + Vector3.up, FindObjectOfType<Camera>().transform.rotation).GetComponent<LootMessage>();
        message.SetText(GetPickupMessage());
        if (pickupClip)
        {
            AudioSource.PlayClipAtPoint(pickupClip, Vector3.zero, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
        }
        Destroy(gameObject);
    }

    private string GetPickupMessage()
    {
        string message = "";
        if(isMissionObjective)
        {
            message += "\nsecured mission objective";
        }
        if(credits>0)
        {
            message += "\nsecured " + credits.ToString() + " credits";
        }
        if(schema.Length>0)
        {
            message += "\nlocated schematic: " + schema;
        }
        return message;
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

    internal void SetRenderer(bool isVisible)
    {
        body.GetComponent<MeshRenderer>().enabled = isVisible;
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
