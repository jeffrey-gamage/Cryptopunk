using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    private TutorialInfo tutorialInfo;
    internal enum Mode
    {
        Deploy,
        Move,
        Attack,
        Breach,
        Proc,
        Wait,
    }
    internal Mode mode = Mode.Deploy;
    internal Mode waitingTo;
    public static DungeonManager instance;
    internal DungeonGrid grid;
    internal GridGenerator generator;

    [SerializeField] Color AttackPreview;
    [SerializeField] Color BreachPreview;
    [SerializeField] Color MovePreview;

    [SerializeField] List<Program> playerPrograms;
    [SerializeField] List<EnemyProgram> enemyPrograms;
    [SerializeField] internal List<SecurityNode> securityNodes;
    [SerializeField] internal List<Hackable> hackableObjects;
    [SerializeField] internal List<Terminal> terminals;
    [SerializeField] internal List<SwitchBridge> switchBridges;

    public int maxTurns;
    public int currentTurn = 0;
    public List<int> reinforcementTurns;
    [SerializeField] int[] reinforcementLineup;
    private int reinforcementIndex = 0;

    public bool isPlayerTurn = true;
    internal Procable activePlugin;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        hackableObjects = new List<Hackable>();
        terminals = new List<Terminal>();
        tutorialInfo = FindObjectOfType<TutorialInfo>();
        grid = FindObjectOfType<DungeonGrid>();
        if (tutorialInfo)
        {
            generator = new GridGenerator(1, 0);
        }
        else
        {
            generator = TryMakeDungeonUntilSuccessful();
        }
        int[][] gridPlan = generator.GetGrid();
        SetUpCamera(gridPlan.Length);
        grid.GenerateGrid(gridPlan);
        grid.GenerateSwitchTilesOn(generator.GetSwitchTilesOn());
        grid.GenerateSwitchTilesOff(generator.GetSwitchTilesOff());
        grid.GenerateRamps(generator.GetRamps());
        grid.GenerateFirewalls(generator.GetFirewalls());
        grid.GenerateDefenses(generator.GetDefences());
        grid.GenerateTerminals(generator.GetTerminals());
        grid.GenerateSwitches(generator.GetSwitches());
        grid.GenerateSecurityHubs(generator.GetHubs());
        if(tutorialInfo)
        {
            AssignEnemyTypes(ref generator,tutorialInfo.GetEnemies());
        }
        foreach (Room room in generator.rooms)
        {
            List<EnemyProgram> enemiesInRoom = grid.GenerateEnemies(room.GetEnemies());
            grid.AssignPatrolRoutes(ref enemiesInRoom, room.patrolRoutes);
        }
        grid.GeneratePorts(generator.GetPorts());
        grid.AssignTerminalControl(generator.GetTerminalControlledObjects());
        foreach (Room room in generator.rooms)
        {
            if(room.isControlledByInternalTerminal)
            {
                room.ConnectTerminal(ref terminals);
            }
            room.ConnectSwitches();
        }

        grid.PlaceLoot(generator.GetLoot());
        grid.PlaceObjective(generator.getMissionObj());
        if (tutorialInfo)
        {
            CreatePlayerPrograms(tutorialInfo.tutorialPrograms);
        }
        else
        {
            CreatePlayerPrograms(MissionStatus.instance.selectedPrograms);
        }
        grid.CreateDeploymentZone(generator.GetDeploymentArea());
        FindObjectOfType<CameraContol>().Configure();
        grid.ExploreStartingArea(generator.GetDeploymentArea());
        PrepareNextDeployment();
    }

    private GridGenerator TryMakeDungeonUntilSuccessful()
    {
        try
        {
            return new GridGenerator(GetDungeonSize(FindObjectOfType<MissionStatus>().GetTotalBudget()), 0);
        }
        catch(ClosedDungeonException e)
        {
            return TryMakeDungeonUntilSuccessful();
        }
    }

    private int GetDungeonSize(int missionBudget)
    {
        return missionBudget/3;
    }

    private void AssignEnemyTypes(ref GridGenerator generator, Vector3Int[] predefinedEnemyTypes)
    {
        foreach(Room room in generator.rooms)
        {
            room.AssignEnemyTypes(predefinedEnemyTypes);
        }
    }

    private void CreatePlayerPrograms(GameObject[] selectedPrograms)
    {
        playerPrograms = new List<Program>();
        for(int i=0;i<selectedPrograms.Length;i++)
        {
            if (selectedPrograms[i] != null)
            {
                Program newProgram = Instantiate(selectedPrograms[i]).GetComponent<Program>();
                if(!FindObjectOfType<TutorialInfo>())
                {
                    foreach(GameObject plugin in MissionStatus.instance.selectedPlugins[i])
                    {
                        Plugin newPlugin = Instantiate(plugin, newProgram.transform).GetComponent<Plugin>();
                        newProgram.plugins.Add(newPlugin);
                    }
                }
                playerPrograms.Add(newProgram);
            }
        }
    }

    internal bool CanCollectLoot(DungeonTile lootTile)
    {
        foreach(EnemyProgram enemy in enemyPrograms)
        {
            if(enemy.myTile==lootTile&!enemy.IsControlledByPlayer())
            {
                return false;
            }
        }
        foreach (Hackable security in hackableObjects)
        {
            if (security.myTile == lootTile & security.isEnabled)
            {
                return false;
            }
        }
        return true;
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
    internal List<EnemyProgram> GetEnemyPrograms()//all enemy programs, including hacked ones
    {
        return enemyPrograms;
    }

    internal List<Program> GetAIControlledPrograms()//only get enemy programs not currently hacked
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
                MissionStatus.instance.outcome = MissionStatus.MissionOutcome.timeout;
                SceneManager.LoadScene("results");
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
            foreach(EnemyProgram program in GetAIControlledPrograms())
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
        foreach(EnemyProgram program in GetAIControlledPrograms())
        {
            if(!program.hasMoved)
            {
                program.ExecuteAIMovement();
                allActionsComplete = false;
                break;
            }
            else if(!program.hasUsedAction)
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

    internal void RightClickTile(DungeonTile dungeonTile)
    {
        if (mode == Mode.Move)
        {
            if (Program.selectedProgram && Program.selectedProgram.IsControlledByPlayer())
            {
                Program.selectedProgram.AttemptMove(dungeonTile);
            }
        }
        else if(mode==Mode.Proc)
        {
            if (Program.selectedProgram && Program.selectedProgram.IsControlledByPlayer())
            {
                activePlugin.Proc(dungeonTile);
            }
        }
    }

    internal void LeftClickTile(DungeonTile dungeonTile)
    {
        if (mode == Mode.Deploy)
        {
            DeploySelected(dungeonTile);
            PrepareNextDeployment();
            if (!Program.selectedProgram)
            {
                ResumeRun();
            }
        }
    }

    private void ResumeRun()
    {
        UpdateVisibility();
        mode = Mode.Move;
    }

    internal void DeploySelected(DungeonTile dungeonTile)
    {
        if (Mathf.Abs(DeploymentZone.instance.myCoords.x - dungeonTile.xCoord) <= DeploymentZone.instance.range &&
            Mathf.Abs(DeploymentZone.instance.myCoords.z - dungeonTile.zCoord) <= DeploymentZone.instance.range &&
            grid.CanDeployHere(dungeonTile))
        {
            Program.selectedProgram.myTile = dungeonTile;
            Program.selectedProgram.transform.position = dungeonTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying(),false);
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
        enemy.SetTile(dungeonTile);
        enemy.transform.position = dungeonTile.GetOccupyingCoordinates(enemy.IsFlying(),false);
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
            if(playerPrograms.Count==0)
            {
                MissionStatus.instance.outcome = MissionStatus.MissionOutcome.eliminated;
                SceneManager.LoadScene("results");
            }
        }
        else 
        {
            enemyPrograms.Remove((EnemyProgram)program);
        }
    }
}
