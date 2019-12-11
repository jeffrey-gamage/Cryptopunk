using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private TutorialInfo tutorialInfo;
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
    [SerializeField] bool IsTutorial;
    [SerializeField] Color BreachPreview;
    [SerializeField] Color MovePreview;

    [SerializeField] List<Program> playerPrograms;
    [SerializeField] List<EnemyProgram> enemyPrograms;
    [SerializeField] internal List<Hackable> hackableObjects;



    [SerializeField] internal List<Terminal> terminals;
    public bool isPlayerTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        hackableObjects = new List<Hackable>();
        terminals = new List<Terminal>();
        if(IsTutorial)
        {
            tutorialInfo = FindObjectOfType<TutorialInfo>();
        }
        grid = FindObjectOfType<DungeonGrid>();
        GridGenerator generator;
        if (IsTutorial)
        {
            generator = new TutorialGridGenerator();
        }
        else
        {
            generator = new GridGenerator(24, 3, 3);
        }
        int[][] gridPlan = generator.GetGrid();
        SetUpCamera(gridPlan.Length);
        grid.GenerateGrid(gridPlan);
        grid.GenerateRamps(generator.GetRamps());
        if (!IsTutorial)
        {
            grid.GenerateFirewalls(generator.GetFirewalls());
            grid.GenerateTerminals(generator.GetTerminals());
            grid.AssignControl(generator.GetTerminalControlledObjects());
        }
        else
        {
            grid.GenerateFirewalls(tutorialInfo.GetFirewallLocations());
            grid.GenerateTerminals(tutorialInfo.GetTerminalInfo());
            grid.AssignControl(tutorialInfo.GetTerminalControlAssignments());
        }
        //grid.GenerateEnemies();
        grid.CreateDeploymentZone(generator.GetStart());
        PrepareNextDeployment();
    }

    private static void SetUpCamera(int gridSize)
    {
        CameraContol cameraContol = FindObjectOfType<CameraContol>();
        cameraContol.center= new Vector3(gridSize / 2, 0, gridSize / 2);
        cameraContol.transform.position = cameraContol.center;
        cameraContol.maxPan = gridSize * 2 / 3;
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
    internal void DeployFromPort(DungeonTile portTile, Program program)
    {
        mode = Mode.Deploy;
        playerPrograms.Add(program);
        DungeonManager.instance.grid.RestrictDeployment(portTile);
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
            turnsLeft--;
            foreach(Program program in GetPlayerControlledPrograms())
            {
                program.OnStartTurn();
            }
        }
        else
        {
            foreach(Terminal terminal in terminals)
            {
                terminal.OnStartTurn();
            }
            foreach(Hackable hackableObject in hackableObjects)
            {
                hackableObject.OnStartTurn();
            }
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
        if(Program.selectedProgram&&mode!=Mode.Deploy)
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
                ResumeRun();
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

    private void ResumeRun()
    {
        UpdateVisibility();
        mode = Mode.Move;
        foreach (Program program in playerPrograms)
        {
            if (program.size > 0)//check if the program has had start called already
            {
                program.OnStartTurn();
            }
        }
    }

    internal void DeploySelected(DungeonTile dungeonTile)
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
