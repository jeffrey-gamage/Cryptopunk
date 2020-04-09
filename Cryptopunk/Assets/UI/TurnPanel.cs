using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] GameObject endTurnButton;
    [SerializeField] Text actionWarning;
    [SerializeField] Text turnCounter;
    [SerializeField] string reinforcementCountdownText = " turns until security is increased";
    [SerializeField] string lastTurnBeforeText = "Last turn before security is increased";
    [SerializeField] string finalCountdownText = " turns until connection is blocked";
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
        if (DungeonManager.instance.reinforcementTurns.Count > 0)
        {
            int turnsUntilReinforcement = DungeonManager.instance.reinforcementTurns[0]+1 - DungeonManager.instance.currentTurn;
            if (turnsUntilReinforcement>1)
            {
                turnCounter.text = turnsUntilReinforcement.ToString() + reinforcementCountdownText;
            }
            else
            {
                turnCounter.text = lastTurnBeforeText;
            }
        }
        else
        {
            int turnsUntilShutdown = DungeonManager.instance.maxTurns+1 - DungeonManager.instance.currentTurn;
            if(turnsUntilShutdown>1)
            {
                turnCounter.text = turnsUntilShutdown.ToString() + finalCountdownText;
            }
        else
            {
                turnCounter.text = lastTurnText;
            }
        }
    }
    public void EndTurn()
    {
        DungeonManager.instance.EndPlayerTurn();
    }
}
