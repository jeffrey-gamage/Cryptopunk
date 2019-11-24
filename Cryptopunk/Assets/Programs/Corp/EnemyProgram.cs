﻿using System;
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
    internal bool hasMoved = false;
    private bool isActiveAI = false;
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
        if (DungeonManager.instance.mode!=DungeonManager.Mode.Wait&&hasMoved&&isActiveAI&& base.movePath.Count == 0)
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

    private bool CanSee(Program program)
    {
        if(program.IsStealthed())
        {
            return DungeonManager.instance.grid.TileDistance(myTile, program.myTile) <= 1;
        }
        return DungeonManager.instance.grid.TileDistance(myTile, program.myTile) <= sight;
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
        Debug.Log("State= " + myState.ToString());
        hasMoved = true;
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
        isActiveAI=true;
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
}
