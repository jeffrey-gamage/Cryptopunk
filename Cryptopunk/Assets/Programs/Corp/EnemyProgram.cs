using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProgram : Program
{
    private static readonly int maxPath = 16;
    private enum State{
        Patrol,
        Search,
        Attack
    }
    private State myState;
    private List<GameObject> lineOfSightIndicators;
    [SerializeField] GameObject sightIndicator;
    [SerializeField] float sightPreviewOffset = 0.11f;
    private int nextWaypointIndex = 0;
    [SerializeField] internal List<DungeonTile> waypoints; //TODO: remove serialization once testing is finished
    private Hackable hackable;
    internal bool hasMoved = false;
    private bool isActiveAI = false;

    internal void InitializeReinforcement()
    {
        myState = State.Search;
        waypoints = new List<DungeonTile>();
        waypoints.Add(myTile);
    }

    override internal void Start()
    {
        base.Start();
        hackable = GetComponent<Hackable>();
        lineOfSightIndicators = new List<GameObject>();
    }
    // Update is called once per frame
    override internal void Update()
    {
        base.Update();
        if(myTile)
        {
            myRenderer.enabled = myTile.isVisible;
            myIcon.enabled = myTile.isVisible;
            hackable.myTile = myTile;
        }
        if (DungeonManager.instance.mode!=DungeonManager.Mode.Wait&&hasMoved&&isActiveAI&&base.movePath.Count == 0)
        {
            isActiveAI = false;
            DungeonManager.instance.TakeNextAIAction();
        }
    }
    internal override void OnStartTurn()
    {
        hasMoved = false;
        base.OnStartTurn();
        GetComponent<Hackable>().OnStartTurn();
        if(myState == State.Attack)
        {
            myState = State.Search;
            nextWaypointIndex = 0;
            waypoints[0] = myTile;
        }
        foreach(Program program in DungeonManager.instance.GetPlayerControlledPrograms())
        {
            if (CanSee(program))
            {
                myState = State.Attack;
            }
        }
    }


    override internal void Damage(int damageAmount)
    {
        GetComponent<Hackable>().Disrupt(damageAmount);
        base.Damage(damageAmount);
    }

    internal void ExecuteAIMovement()
    {
        isActiveAI = true;
        if(myState==State.Patrol)
        {
            Patrol();
        }
        else if(myState==State.Search)
        {
            Search();
        }
        else if(myState==State.Attack)
        {
            MoveIntoRange();
        }
        Debug.Log(gameObject.name+": State= " + myState.ToString());
        hasMoved = true;
    }

    internal void SetWaypoints(Vector3Int[] waypointCoords)
    {
        waypoints = new List<DungeonTile>();
        foreach(Vector3Int waypointCoord in waypointCoords)
        {
            waypoints.Add(DungeonManager.instance.grid.GetTile(waypointCoord.x,waypointCoord.z));
        }
    }

    private void MoveIntoRange()
    {
        List<Program> hostilePrograms = DungeonManager.instance.GetPlayerControlledPrograms();
        Program target=null;
        foreach(Program program in hostilePrograms)
        {
            if (CanSee(program))
            {
                if (!target || (DungeonManager.instance.grid.TileDistance(program.myTile, myTile) < DungeonManager.instance.grid.TileDistance(target.myTile, myTile)))
                {
                    target = program;
                    Debug.Log("New Target selected");
                }
            }
        }
        if(target)
        {
            Debug.Log("Moving to close with target");
            NavigateTowards(DungeonManager.instance.grid.GetNearestTileInRange(this, target.myTile, range, movesLeft));
        }
    }
    internal override void OnMouseDown()
    {
        base.OnMouseDown();
        if (selectedProgram == this&&!hackable.IsHacked())
        {
            GenerateLineOfSightIndicators();
        }
    }

    private void GenerateLineOfSightIndicators()
    {
        ClearSightPreview();
        if (!IsControlledByPlayer())
        {
            foreach (DungeonTile tile in DungeonManager.instance.grid.GetAllSeenTiles(this))
            {
                lineOfSightIndicators.Add(CreateLineOfSightIndicator(tile));
            }
        }
    }

    private GameObject CreateLineOfSightIndicator(DungeonTile tile)
    {
        return Instantiate(sightIndicator, tile.GetOccupyingCoordinates(false) + Vector3.up * sightPreviewOffset, tile.getOccupantRotation());
    }

    private void Search()
    {
        if(myTile==waypoints[0])
        {
            Debug.Log("Choosing new search location:");
            waypoints[0] = DungeonManager.instance.grid.GetNewSearchLocation(myTile);
            Debug.Log(waypoints[0].xCoord.ToString() +", "+ waypoints[0].zCoord.ToString());
        }
        NavigateTowards(waypoints[0]);
    }

    private void Patrol()
    {
        if (!(waypoints.Count > 0))
        {
            Debug.Log("Patrol route failed to map");
            waypoints.Add(myTile);
        }
        if (myTile == waypoints[nextWaypointIndex])
        {
            nextWaypointIndex = (nextWaypointIndex + 1) % waypoints.Count;
        }
        NavigateTowards(waypoints[nextWaypointIndex]);
    }

    private void NavigateTowards(DungeonTile goal)
    {
        List<DungeonTile> path = DungeonManager.instance.grid.FindPath(myTile, goal, maxPath, IsFlying());
        if (path.Count > 0)
        {
            if (path.Count > movesLeft)
            {
                AttemptMove(path[movesLeft]);
            }
            else
            {
                AttemptMove(goal);
            }
        }
        else
        {
            myState = State.Search;
            waypoints[0] = myTile;
        }
    }

    internal void ExecuteAIAttack()
    {
        if (hackable.isEnabled)
        {
            isActiveAI = true;
            List<Program> hostilePrograms = DungeonManager.instance.GetPlayerControlledPrograms();
            Program target = null;
            foreach (Program program in hostilePrograms)
            {
                if (CanSee(program))
                {
                    if (!target || (DungeonManager.instance.grid.TileDistance(program.myTile, myTile) < DungeonManager.instance.grid.TileDistance(target.myTile, myTile)))
                    {
                        target = program;
                        Debug.Log("New Target selected");
                    }
                }
            }
            if (target)
            {
                AttemptAttack(target);

            }
            if (!hasAttacked)
            {
                hasAttacked = true;
                isActiveAI = false;
                DungeonManager.instance.TakeNextAIAction();
            }
        }
        else
        {
            hasAttacked = true;
            DungeonManager.instance.TakeNextAIAction();
        }
    }

    internal void ClearSightPreview()
    {
        if (lineOfSightIndicators.Count > 0 && Program.selectedProgram != this)
        {
            foreach (GameObject indicator in lineOfSightIndicators)
            {
                Destroy(indicator);
            }
            lineOfSightIndicators = new List<GameObject>();
        }
    }

    protected override void Die()
    {
        ClearSightPreview();
        base.Die();
    }
}
