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
    [SerializeField] internal int turnsLeft = 10;
    internal DungeonGrid grid;

    [SerializeField] Color AttackPreview;
    [SerializeField] Color BreachPreview;
    [SerializeField] Color MovePreview;

    [SerializeField] List<Program> playerPrograms;
    [SerializeField] List<EnemyProgram> enemyPrograms;
    public bool isPlayerTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        grid = FindObjectOfType<DungeonGrid>();
        int[][] gridPlan = GridGenerator.GenerateGrid(3);
        FindObjectOfType<CameraContol>().transform.position = new Vector3(gridPlan.Length / 2, 0, gridPlan.Length / 2);
        grid.GenerateGrid(gridPlan);
        grid.GenerateRamps();
        grid.GenerateEnemies();
        PrepareNextDeployment();
    }

    internal List<Program> GetPlayerControlledPrograms()
        //return a list of all programs currently controlled by the chosen player
    {
        List<Program> controlledPrograms = new List<Program>();
        foreach(Program program in playerPrograms)
        {
                controlledPrograms.Add(program);
        }
        foreach (Program program in enemyPrograms)
        {
            if (program.IsControlledByPlayer())
            {
                controlledPrograms.Add(program);
            }
        }
        return controlledPrograms;
    }

    internal List<Program> GetAICotrolledPrograms()
    {
        List<Program> controlledPrograms = new List<Program>();
        foreach (Program program in enemyPrograms)
        {
            if (!program.IsControlledByPlayer())
            {
                controlledPrograms.Add(program);
            }
        }
        return controlledPrograms;
    }
    internal bool HasActionsLeft()
    {
        bool hasActionsLeft = false;
        foreach(Program program in playerPrograms)
        {
            if(program.movesLeft>0)
            {
                hasActionsLeft = true;
            }
        }
        foreach(Program program in enemyPrograms)
        {
            if(program.movesLeft>0&&program.IsControlledByPlayer())
            {
                hasActionsLeft = true;
            }
        }
        return hasActionsLeft;
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
            turnsLeft--;
            foreach(Program program in GetPlayerControlledPrograms())
            {
                program.OnStartTurn();
            }
        }
        else
        {
            foreach(EnemyProgram program in GetAICotrolledPrograms())
            {
                program.OnStartTurn();
            }
            TakeNextAIAction();
        }
    }

    internal void TakeNextAIAction()
        //Does the next available AI action, or if none remain, passes turn to the player. Each AI calls this at the conclusion of each of its actions.
    {
        bool allActionsComplete = true;
        foreach(EnemyProgram program in GetAICotrolledPrograms())
        {
            if(!program.hasMoved)
            {
                program.ExecuteAIMovement();
                allActionsComplete = false;
                break;
            }
            else if(!program.hasAttacked)
            {
                program.ExecuteAIAttack();
                allActionsComplete = false;
                break;
            }
        }
        if(allActionsComplete)
        {
            EndTurn();
        }
    }

    internal void PreviewTile(DungeonTile dungeonTile)
    {
        if(Program.selectedProgram&&mode!=Mode.Deploy&&mode!=Mode.Wait)
        {
            PathPreview.instance.DisplayPreview(grid.FindPath(Program.selectedProgram.myTile, dungeonTile, Program.selectedProgram.movesLeft, Program.selectedProgram.IsFlying()));
            if(Program.isTargetingAttack)
            {
                PathPreview.instance.SetColor(AttackPreview);
            }
            else if(Program.isTargetingBreach)
            {
                PathPreview.instance.SetColor(BreachPreview);
            }
            else
            {
                PathPreview.instance.SetColor(MovePreview);
            }
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
        grid.FogOfWarRender(GetPlayerControlledPrograms().ToArray());
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
            if (Program.selectedProgram&&Program.selectedProgram.IsControlledByPlayer())
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
        dungeonTile.Occupy(Program.selectedProgram);
    }

    internal void DeploySecurity(EnemyProgram enemy, DungeonTile dungeonTile)
    {
        enemy.myTile = dungeonTile;
        enemy.transform.position = dungeonTile.GetOccupyingCoordinates(enemy.IsFlying());
        enemyPrograms.Add(enemy);
        dungeonTile.Occupy(enemy);
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
        if (playerPrograms.Contains(program))
        {
            playerPrograms.Remove(program);
        }
        else 
        {
            enemyPrograms.Remove((EnemyProgram)program);
        }
    }
}
