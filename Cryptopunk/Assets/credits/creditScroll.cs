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
    [SerializeField] string[] creditText;

    private int nextCreditIndex = 0;
    private float timeSinceLastSpawn = 0f;
    private List<RectTransform> credits;

    private void Start()
    {
        credits = new List<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
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
            if (creditText.Length > nextCreditIndex)
            {
                timeSinceLastSpawn = maxSpawnRate;
                Text newCredit = Instantiate(creditPrefab, spawnPoint.transform).GetComponent<Text>();
                newCredit.text = creditText[nextCreditIndex];
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
