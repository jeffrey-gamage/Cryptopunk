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
        Wait,
    }
    internal Mode mode = Mode.Deploy;
    internal Mode waitingTo;
    public static DungeonManager instance;
    internal DungeonGrid grid;

    [SerializeField] List<Program> playerPrograms;
    [SerializeField] List<Program> enemyPrograms;
    public bool isPlayerTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<DungeonGrid>();
        int[][] gridPlan = GridGenerator.GenerateGrid(3);
        FindObjectOfType<CameraContol>().transform.position = new Vector3(gridPlan.Length / 2, 0, gridPlan.Length / 2);
        grid.GenerateGrid(gridPlan);
        grid.GenerateRamps();
        grid.GenerateEnemies();
        instance = this;
        PrepareNextDeployment();
    }

    private void PrepareNextDeployment()
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

    internal void UpdateVisibility()
    {
        grid.FogOfWarRender(playerPrograms.ToArray());
    }

    internal void SelectTile(DungeonTile dungeonTile)
    {
        if(mode==Mode.Deploy)
        {
            DeploySelected(dungeonTile);
            PrepareNextDeployment();
            if (!Program.selectedProgram)
            {
                BeginRun();
            }
        }
        else if (mode==Mode.Move)
        {
            if (Program.selectedProgram&&Program.selectedProgram.IsControlled())
            {
                Program.selectedProgram.AttemptMove(dungeonTile);
            }
        }
    }

    private void BeginRun()
    {
        UpdateVisibility();
        mode = Mode.Move;
        foreach (Program program in playerPrograms)
        {
            program.OnStartTurn();
        }
    }

    internal static void DeploySelected(DungeonTile dungeonTile)
    {
        Program.selectedProgram.myTile = dungeonTile;
        Program.selectedProgram.transform.position = dungeonTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying());
        Program.selectedProgram.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    internal static void DeploySecurity(EnemyProgram enemy, DungeonTile dungeonTile)
    {
        enemy.myTile = dungeonTile;
        enemy.transform.position = dungeonTile.GetOccupyingCoordinates(enemy.IsFlying());
    }


    internal void Wait()
    {
        waitingTo = mode;
        mode = Mode.Wait;
    }
    internal void Resume()
    {
        if (mode == Mode.Wait)
        {
            mode = waitingTo;
        }
        else
        {
            Debug.LogWarning("Tried to resume while not waiting on animation");
        }
    }

    internal bool IsPlayers(Program program)
    {
        return playerPrograms.Contains(program);
    }

    internal void RemoveProgram(Program program)
    {
        playerPrograms.Remove(program);
        enemyPrograms.Remove(program);
    }
}
