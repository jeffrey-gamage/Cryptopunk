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
    private Collider myCollider;
    internal bool hasMoved = false;
    private bool isActiveAI = false;
    [SerializeField]internal int difficultyRating;

    internal void InitializeReinforcement()
    {
        myState = State.Search;
        waypoints = new List<DungeonTile>();
        waypoints.Add(myTile);
    }

    override internal void Start()
    {
        base.Start();
        myCollider = GetComponent<Collider>();
        hackable = GetComponent<Hackable>();
        lineOfSightIndicators = new List<GameObject>();
    }
    // Update is called once per frame
    override internal void Update()
    {
        base.Update();
        if(myTile)
        {
            myRenderer.enabled = myTile.IsVisible()&&myTile.IsFinishedRevealAnimation();
            myCollider.enabled = myTile.IsVisible() && myTile.IsFinishedRevealAnimation();
            myIcon.enabled = myTile.IsVisible() && myTile.IsFinishedRevealAnimation();
            hackable.myTile = myTile;
        }
        if (DungeonManager.instance.mode!=DungeonManager.Mode.Wait&&hasMoved&&isActiveAI&& movePath.Count == 0)
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
    }

    private void UpdateState()
    {
        if (myState == State.Attack)
        {
            myState = State.Search;
            nextWaypointIndex = 0;
            waypoints.Clear();
            waypoints.Add(myTile);
        }
        foreach (Program program in DungeonManager.instance.GetPlayerControlledPrograms())
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
        UpdateState();
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
                }
            }
        }
        if(target)
        {
            NavigateTowards(DungeonManager.instance.grid.GetNearestTileInRange(this, target.myTile, baseRange, movesLeft));
        }
    }
    internal override void OnMouseOver()
    {
        base.OnMouseOver();
        if (Input.GetMouseButtonDown(0)&&selectedProgram == this&&!hackable.IsHacked())
        {
            GenerateLineOfSightIndicators();
        }
    }

    internal void GenerateLineOfSightIndicators()
    {
        ClearSightPreview();
        if (!IsControlledByPlayer()||hackable.IsHacked())
        {
            foreach (DungeonTile tile in DungeonManager.instance.grid.GetAllSeenTiles(this))
            {
                lineOfSightIndicators.Add(CreateLineOfSightIndicator(tile));
            }
        }
    }

    private GameObject CreateLineOfSightIndicator(DungeonTile tile)
    {
        return Instantiate(sightIndicator, tile.GetOccupyingCoordinates(false,true) + Vector3.up * sightPreviewOffset, tile.getPreviewRotation());
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
            Debug.Log("Patrol route failed to map at coords: "+myTile.xCoord.ToString()+", "+myTile.zCoord.ToString());
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
        if (lineOfSightIndicators.Count > 0)
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

    internal void SetTile(DungeonTile dungeonTile)
    {
        myTile = dungeonTile;
        hackable = GetComponent<Hackable>();
        hackable.myTile = dungeonTile;
    }
}
