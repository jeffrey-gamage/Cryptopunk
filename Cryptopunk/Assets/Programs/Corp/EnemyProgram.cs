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
    private int nextWaypointIndex = 0;
    internal List<DungeonTile> waypoints;
    private MeshRenderer myRenderer;
    private Hackable hackable;
    override internal void Start()
    {
        base.Start();
        myRenderer = GetComponent<MeshRenderer>();
        hackable = GetComponent<Hackable>();
    }
    // Update is called once per frame
    override internal void Update()
    {
        base.Update();
        if(myTile)
        {
            myRenderer.enabled = myTile.isVisible;
            hackable.myTile = myTile;
        }
    }
    internal override void OnStartTurn()
    {
        base.OnStartTurn();
        GetComponent<Hackable>().OnStartTurn();
        if(myState == State.Attack)
        {
            myState = State.Search;
            nextWaypointIndex = 0;
            waypoints[0] = myTile;
        }
        foreach(Program program in DungeonManager.instance.GetControlledPrograms(true))
        {
            if(DungeonManager.instance.grid.TileDistance(myTile,program.myTile)<=sight)
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

    internal void ExecuteAIBehaviour()
    {
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
    }

    private void MoveIntoRange()
    {
        List<Program> enemyPrograms = DungeonManager.instance.GetControlledPrograms(true);
        Program target=null;
        foreach(Program program in enemyPrograms)
        {
            if(!target||(DungeonManager.instance.grid.TileDistance(program.myTile,myTile)< DungeonManager.instance.grid.TileDistance(target.myTile, myTile)))
            {
                target = program;
            }
        }
        if(target)
        {
            AttemptMove(DungeonManager.instance.grid.GetNearestTileInRange(myTile, target.myTile, range, movesLeft));
        }
    }

    private void Search()
    {
        if(myTile==waypoints[0])
        {
            waypoints[0] = DungeonManager.instance.grid.GetNewSearchLocation(myTile);
        }
        NavigateTowards(waypoints[0]);
    }

    private void Patrol()
    {
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
}
