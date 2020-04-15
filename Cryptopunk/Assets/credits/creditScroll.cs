using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creditScroll : MonoBehaviour
{
    [SerializeField] float maxSpawnRate;
    [SerializeField] float scrollSpeed;
    [SerializeField] GameObject creditPrefab;
    private Canvas canvas;
    [SerializeField] RectTransform spawnPoint;
    [SerializeField] RectTransform despawnPoint;

    [SerializeField] TextAsset creditList;
    private List<string> creditTexts;
    private int nextCreditIndex = 0;
    private float timeSinceLastSpawn = 0f;
    private List<RectTransform> credits;

    private void Start()
    {
        credits = new List<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        creditTexts = ParseCredits();

    }

    private List<string> ParseCredits()
    {
        List<string> parsedCredits = new List<string>();
        string creditListAsString = creditList.text;
        string nextCredit = GetNextLine(ref creditListAsString);
        while(!nextCredit.Contains("++ENDFILE++"))
        {
            parsedCredits.Add(nextCredit);
            nextCredit = GetNextLine(ref creditListAsString);
        }
        return parsedCredits;
    }
    private static string GetNextLine(ref string creditsText)
    {
        int indexOfNextNewline = creditsText.IndexOf("\n");
        string nextLine;
        if (indexOfNextNewline < 0)
        {
            nextLine = "++ENDFILE++";
        }
        else
        {
            nextLine = creditsText.Substring(0, indexOfNextNewline-1);
        }
        creditsText = creditsText.Substring(indexOfNextNewline + 1);
        return nextLine;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnCredits();
        ScrollAndDespawnCredits();
    }

    private void SpawnCredits()
    {
        if (timeSinceLastSpawn <= 0f)
        {
            if (creditTexts.Count > nextCreditIndex)
            {
                timeSinceLastSpawn = maxSpawnRate;
                Text newCredit = Instantiate(creditPrefab, spawnPoint.transform).GetComponent<Text>();
                newCredit.text = creditTexts[nextCreditIndex];
                nextCreditIndex++;
                credits.Add(newCredit.GetComponent<RectTransform>());
            }
        }
        timeSinceLastSpawn -= Time.deltaTime;
    }

    private void ScrollAndDespawnCredits()
    {
        if (credits.Count > 0)
        {
            for (int i = 0; i < credits.Count; i++)
            {
                if (credits[i].position.y > despawnPoint.position.y)
                {
                    Destroy(credits[i].gameObject);
                    credits.RemoveAt(i);
                    i--;
                }
                else
                {
                    credits[i].position += Vector3.up * scrollSpeed * Time.deltaTime;
                }
            }
        }
    }
}
