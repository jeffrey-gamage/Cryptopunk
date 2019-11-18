using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] Program[] playerPrograms;
    [SerializeField] Program[] enemyPrograms;
    private bool isPlayerTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        DungeonGrid grid = FindObjectOfType<DungeonGrid>();
        int[][] gridPlan = GridGenerator.GenerateGrid(3);
        grid.GenerateGrid(gridPlan);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if(isPlayerTurn)
        {
            foreach(Program program in playerPrograms)
            {
                program.OnStartTurn();
            }
        }
        else
        {
            foreach(Program program in enemyPrograms)
            {
                program.OnStartTurn();
                //TODO program.executeAITurn;
            }
            EndTurn();
        }
    }

    public void EndPlayerTurn()
    {
        if(isPlayerTurn)
        {
            EndTurn();
        }
    }
}
