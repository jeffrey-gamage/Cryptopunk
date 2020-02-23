using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    internal GameObject item;
    [SerializeField] Image icon;
    [SerializeField] Text programName;
    [SerializeField] Text size;
    [SerializeField] Text speed;
    [SerializeField] Text sight;
    [SerializeField] Text power;
    [SerializeField] Text range;
    [SerializeField] Text breach;
    [SerializeField] Text keywords;

    [SerializeField] Button buyButton;
    private Text buyButtonText;

    [SerializeField] int minBaseCost = 20;//range is inclusive on bottom, will multiply by 5
    [SerializeField] int maxBaseCost = 41;//range is exclusive on top, will multiply by 5
    internal int cost;
    
    private GameObject itemForSale;
    // Start is called before the first frame update
    private void Start()
    {
        buyButtonText = buyButton.GetComponentInChildren<Text>();
        cost = RandomCost();
    }

    private int RandomCost()
    {
        return UnityEngine.Random.Range(minBaseCost, maxBaseCost) * 5;
    }

    private void Update()
    {
        buyButtonText.text = "Buy: " +cost.ToString() + " credits";
    }

    public void BuyItem()
    {
        if (PersistentState.instance.credits >= cost)
        {
            FindObjectOfType<shop>().Buy(this);
        }
        else
        {
            FindObjectOfType<shop>().OverBudgetFeedback();
        }
    }
    
    internal void SetSchema(GameObject item)
    {
        this.item = item;
        PlayerProgram program = item.GetComponent<PlayerProgram>();
        icon.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
        programName.text = item.name;
        size.text = "size: "+program.size.ToString();
        speed.text = "speed: "+program.speed.ToString();
        sight.text = "sight: "+program.sight.ToString();
        power.text = "power: "+program.power.ToString();
        range.text = "range: "+program.range.ToString();
        breach.text = "breach: "+program.breach.ToString();
        keywords.text = "";
        foreach(string keyword in program.keywords)
        {
            keywords.text += keyword + "\n";
        }
    }
}
