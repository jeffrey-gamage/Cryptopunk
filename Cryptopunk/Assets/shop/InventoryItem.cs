using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    internal GameObject item;
    [SerializeField] Image icon;
    [SerializeField] Text schemaName;
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
    [SerializeField] int maxBaseCost = 31;//range is exclusive on top, will multiply by 5
    internal int cost;
    
    private GameObject itemForSale;
    // Start is called before the first frame update
    private void Start()
    {
        buyButtonText = buyButton.GetComponentInChildren<Text>();
    }

    internal void RandomizeCost()
    {
        cost= UnityEngine.Random.Range(minBaseCost, maxBaseCost) * 5;
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
        if (program)
        {
            icon.sprite = item.GetComponentInChildren<SpriteRenderer>().sprite;
            schemaName.text = item.name+ " [Program]";
            size.text = "size: " + program.GetSize().ToString();
            speed.text = "speed: " + program.GetSpeed().ToString();
            sight.text = "sight: " + program.GetSight().ToString();
            power.text = "power: " + program.GetPower().ToString();
            range.text = "range: " + program.GetRange().ToString();
            breach.text = "breach: " + program.GetBreach().ToString();
            keywords.text = "";
            foreach (string keyword in program.GetKeywords())
            {
                keywords.text += keyword + "\n";
            }
        }
        else
        {
            Plugin plugin = item.GetComponent<Plugin>();
            {
                icon.sprite = plugin.icon;
                schemaName.text = item.name + " [Plugin]";
                size.text = "size: +" + plugin.size.ToString();
                speed.text = "speed: +" + plugin.speed.ToString();
                sight.text = "sight: +" + plugin.sight.ToString();
                power.text = "power: +" + plugin.power.ToString();
                range.text = "range: +" + plugin.range.ToString();
                breach.text = "breach: +" + plugin.breach.ToString();
                keywords.text = "";
                foreach (string keyword in plugin.keywords)
                {
                    keywords.text += keyword + "\n";
                }
            }
        }
    }
}
