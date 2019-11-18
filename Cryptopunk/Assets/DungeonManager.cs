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
    private DungeonGrid grid;

    [SerializeField] Program[] playerPrograms;
    [SerializeField] Program[] enemyPrograms;
    public bool isPlayerTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<DungeonGrid>();
        int[][] gridPlan = GridGenerator.GenerateGrid(3);
        grid.GenerateGrid(gridPlan);
        instance = this;
        PrepareDeployment();
    }

    private void PrepareDeployment()
    {
        Program toDeploy = null;
        foreach(Program program in playerPrograms)
        {
            if(!program.myTile)
            {
                toDeploy = program;
            }
        }
        Program.selectedProgram = toDeploy;
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
            Program.selectedProgram.myTile = dungeonTile;
            Program.selectedProgram.transform.position = dungeonTile.transform.position;
            Program.selectedProgram.gameObject.GetComponent<MeshRenderer>().enabled = true;
            PrepareDeployment();
            if(!Program.selectedProgram)
            {
                mode = Mode.Move;
            }
        }
        if(mode==Mode.Move)
        {
            Program.selectedProgram.AttemptMove(dungeonTile);
        }
    }
}
