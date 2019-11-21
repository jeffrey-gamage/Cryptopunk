using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] GameObject endTurnButton;
    [SerializeField] Text actionWarning;
    [SerializeField] Text turnCounter;
    [SerializeField] string turnCounterText = " turns until connection is blocked";
    [SerializeField] string lastTurnText = "Last turn before connection is blocked";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        endTurnButton.SetActive(DungeonManager.instance.isPlayerTurn);
        actionWarning.enabled = DungeonManager.instance.isPlayerTurn && DungeonManager.instance.HasActionsLeft();
        if(DungeonManager.instance.turnsLeft>0)
        {
            turnCounter.text = DungeonManager.instance.turnsLeft.ToString() + turnCounterText;
        }
        else
        {
            turnCounter.text = lastTurnText;
        }
    }
    public void EndTurn()
    {
        DungeonManager.instance.EndPlayerTurn();
    }
}
