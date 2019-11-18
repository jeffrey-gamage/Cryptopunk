using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    internal enum Mode
    {
        Deploy,
        Move,
        Attack,
        Breach,
        Wait
    }
    internal Mode mode = Mode.Deploy;
    public static DungeonManager instance;

    [SerializeField] Program[] playerPrograms;
    [SerializeField] Program[] enemyPrograms;
    public bool isPlayerTurn = true;
    public bool isDeployment= true;
    // Start is called before the first frame update
    void Start()
    {
        DungeonGrid grid = FindObjectOfType<DungeonGrid>();
        int[][] gridPlan = GridGenerator.GenerateGrid(3);
        grid.GenerateGrid(gridPlan);
        instance = this;
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

    internal void SelectTile(DungeonTile dungeonTile)
    {
        if(mode==Mode.Deploy)
        {
            //TODO: Get program from deployement queue
            //assign to selected tile
            //remove from deployment queue
            //if deployment queue is empty, switch mode to move
        }
    }
}
