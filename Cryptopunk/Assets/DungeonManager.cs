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
    internal DungeonGrid grid;

    [SerializeField] Color AttackPreview;
    [SerializeField] bool IsTutorial;
    [SerializeField] Color BreachPreview;
    [SerializeField] Color MovePreview;

    [SerializeField] List<Program> playerPrograms;
    [SerializeField] List<EnemyProgram> enemyPrograms;
    [SerializeField] internal List<SecurityNode> securityNodes;
    [SerializeField] internal List<Hackable> hackableObjects;
    [SerializeField] internal List<Terminal> terminals;

    public int maxTurns;
    public int currentTurn = 0;
    public List<int> reinforcementTurns;
    [SerializeField] int[] reinforcementLineup;
    private int reinforcementIndex = 0;

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
            grid.GenerateDefenses(generator.GetDefences());
            grid.GenerateTerminals(generator.GetTerminals());
            grid.GenerateSecurityHubs(generator.GetHubs());
            grid.GenerateEnemies(generator.GetEnemies());
            grid.GeneratePorts(generator.GetPorts());
            grid.AssignControl(generator.GetTerminalControlledObjects());
            grid.PlaceLoot(generator.GetLoot());
        }
        else
        {
            grid.GenerateFirewalls(tutorialInfo.GetFirewallLocations());
            grid.GenerateDefenses(tutorialInfo.GetDefencePlacements());
            grid.GenerateTerminals(tutorialInfo.GetTerminalInfo());
            grid.GenerateSecurityHubs(tutorialInfo.GetHubs());
            grid.GenerateEnemies(tutorialInfo.GetEnemies());
            for(int i=0;i<enemyPrograms.Count;i++)
            {
                enemyPrograms[i].SetWaypoints(tutorialInfo.GetPatrolRoute(i));
            }
            grid.GeneratePorts(tutorialInfo.GetPortLocations());
            grid.AssignControl(tutorialInfo.GetTerminalControlAssignments());
            grid.PlaceLoot(tutorialInfo.GetLootPlacements());
        }
        grid.CreateDeploymentZone(generator.GetDeploymentArea());
        grid.ExploreStartingArea(generator.GetStartingArea());
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
        foreach(Program program in playerPrograms)
        {
            if(program.movesLeft>0)
            {
                return true;
            }
        }
        foreach(Program program in enemyPrograms)
        {
            if(program.movesLeft>0&&program.IsControlledByPlayer())
            {
                return true;
            }
        }
        return false;
    }
    internal void DeployFromPort(DungeonTile portTile, GameObject program)
    {
        mode = Mode.Deploy;
        Program newProgram = Instantiate(program).GetComponent<Program>();
        playerPrograms.Add(newProgram);
        DungeonManager.instance.grid.CreateDeploymentZone(new Vector3Int(portTile.xCoord,0,portTile.zCoord));
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
        if(!toDeploy)
        {
            Destroy(DeploymentZone.instance.gameObject);
        }
    }

    void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if(isPlayerTurn)
        {
            currentTurn++;
            foreach(Program program in GetPlayerControlledPrograms())
            {
                program.OnStartTurn();
            }
        }
        else
        {
            if(currentTurn==maxTurns)
            {
                //TODO: lose state
            }
            else if(reinforcementTurns.Contains(currentTurn))
            {
                reinforcementTurns.Remove(currentTurn);
                DeployReinforcements();
            }
            foreach(Terminal terminal in terminals)
            {
                terminal.OnStartTurn();
            }
            foreach(Hackable hackableObject in hackableObjects)
            {
                if (!hackableObject.myProgram)
                {
                    hackableObject.OnStartTurn();
                }
            }
            foreach(EnemyProgram program in GetAICotrolledPrograms())
            {
                program.OnStartTurn();
            }
            TakeNextAIAction();
        }
    }

    private void DeployReinforcements()
    {
        foreach(SecurityNode node in securityNodes)
        {
            node.DeployReinforcement(grid.enemyPrefabs[reinforcementLineup[reinforcementIndex]]);
            reinforcementIndex = (reinforcementIndex +1)% reinforcementLineup.Length;
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
        if (Mathf.Abs(DeploymentZone.instance.myCoords.x - dungeonTile.xCoord) <= DeploymentZone.instance.range &&
            Mathf.Abs(DeploymentZone.instance.myCoords.z - dungeonTile.zCoord) <= DeploymentZone.instance.range &&
            grid.CanDeployHere(dungeonTile))
        {
            Program.selectedProgram.myTile = dungeonTile;
            Program.selectedProgram.transform.position = dungeonTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying());
            Program.selectedProgram.BeginPlay();
            dungeonTile.Occupy(Program.selectedProgram);
        }
        else
        {
            Debug.Log((Mathf.Abs(DeploymentZone.instance.myCoords.x - dungeonTile.xCoord)).ToString());
            Debug.Log((Mathf.Abs(DeploymentZone.instance.myCoords.z - dungeonTile.zCoord)).ToString());
            Debug.Log(DeploymentZone.instance.range.ToString());
            Debug.Log(grid.CanDeployHere(dungeonTile).ToString());
        }
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
