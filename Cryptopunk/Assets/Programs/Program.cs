﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    protected MeshRenderer myRenderer;
    protected SpriteRenderer myIcon;
    private bool hasBegunPlay = false;
    public bool isAnimating = false;
    public static bool isTargetingAttack = false;
    public static bool isTargetingBreach = false;
    public static float animationSpeed = 2f;
    public static Program selectedProgram;
    [SerializeField] internal int maxSize;
    [SerializeField] internal int power;
    [SerializeField] internal int speed;
    [SerializeField] internal int range;
    [SerializeField] internal int sight;
    [SerializeField] internal int breach;
    [SerializeField] internal List<string> keywords;
    [SerializeField] GameObject myAttack;
    [SerializeField] GameObject myBreach;
    internal DungeonTile myTile;
    internal int size =0;
    internal int movesLeft;
    internal bool hasAttacked;
    internal bool hasBeenSpotted;
    internal List<DungeonTile> movePath;

    protected Material standardMaterial;
    // Start is called before the first frame update
    internal virtual void Start()
    {
        size = maxSize;
        myIcon = GetComponentInChildren<SpriteRenderer>();
        myRenderer = GetComponent<MeshRenderer>();
        standardMaterial = myRenderer.material;
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        HandleMovement();
    }

    private void AttemptCollectLoot()
    {
        if(myTile&&myTile.loot&&IsControlledByPlayer()&&DungeonManager.instance.CanCollectLoot(myTile))
        {
            myTile.loot.Yield();
        }
    }

    private void HandleMovement()
    {
        if (isAnimating)
        {
            Vector3 motion = (myTile.GetOccupyingCoordinates(IsFlying()) - gameObject.transform.position).normalized * animationSpeed * Time.deltaTime;
            if (motion==Vector3.zero||motion.magnitude > (myTile.GetOccupyingCoordinates(IsFlying()) - gameObject.transform.position).magnitude)
            {
                gameObject.transform.position = myTile.GetOccupyingCoordinates(IsFlying());
                if (movePath.Count == 0)
                {
                    isAnimating = false;
                    DungeonManager.instance.Resume();
                }
                else
                {
                    myTile = movePath[0];
                    AttemptCollectLoot();
                    movePath.Remove(myTile);
                    DungeonManager.instance.UpdateVisibility();
                }
                CheckStealth();
            }
            else
            {
                gameObject.transform.position += motion;
            }
        }
    }

    private void CheckStealth()
    {
        List<Program> hostilePrograms;
        if(this.IsControlledByPlayer())
        {
            hostilePrograms = DungeonManager.instance.GetAICotrolledPrograms();
        }
        else
        {
            hostilePrograms = DungeonManager.instance.GetPlayerControlledPrograms();
        }
        foreach(Program program in hostilePrograms)
        {
            if(program.IsStealthed()&&this.CanSee(program))
            {
                program.hasBeenSpotted = true;
            }
            if(this.IsStealthed()&&program.CanSee(this))
            {
                this.hasBeenSpotted = true;
            }
        
        }

    }
    protected bool CanSee(Program program)
    {
        if (program.IsStealthed()&&!keywords.Contains("Sensor"))
        {
            return DungeonManager.instance.grid.TileDistance(myTile, program.myTile) <= 1;
        }
        return DungeonManager.instance.grid.TileDistance(myTile, program.myTile) <= sight&&DungeonManager.instance.grid.IsInLineOfSight(this,program.myTile);
    }

    internal bool IsStealthed()
    {
        return keywords.Contains("Ghost") && !hasAttacked &&!hasBeenSpotted;
    }

    internal bool IsFlying()
    {
        return keywords.Contains("Flying");
    }

    internal virtual void OnStartTurn()
    {
        hasBegunPlay = true;
        hasAttacked = false;
        hasBeenSpotted = false;
        movesLeft = speed;
    }

    internal virtual void OnMouseDown()
    {
        if (isTargetingAttack && !DungeonManager.instance.IsPlayers(this) && Program.selectedProgram.IsControlledByPlayer())
        {
            Program.selectedProgram.AttemptAttack(this);
        }
        else if (isTargetingBreach && !DungeonManager.instance.IsPlayers(this) && Program.selectedProgram.IsControlledByPlayer())
        {
            //breach is handled by hackable component
        }
        else if (DungeonManager.instance.mode != DungeonManager.Mode.Deploy || !hasBegunPlay)//prevent port deployment from moving your programs
        {
            selectedProgram = this;
            FindObjectOfType<PathPreview>().ClearPreview();
            Hackable.selectedObject = GetComponent<Hackable>();
        }
        ClearSightPreviews();
    }

    private void ClearSightPreviews()
    {
        foreach(EnemyProgram enemyProgram in DungeonManager.instance.GetAICotrolledPrograms())
        {
            enemyProgram.ClearSightPreview();
        }
    }
    private void OnMouseOver()
    {
        if (isTargetingAttack && !DungeonManager.instance.IsPlayers(this) && Program.selectedProgram.IsControlledByPlayer())
        {
            DungeonManager.instance.PreviewTile(this.myTile);
        }
    }

    internal bool IsControlledByPlayer()
    {
            return DungeonManager.instance.IsPlayers(this) || (GetComponent<Hackable>() && GetComponent<Hackable>().IsHacked());
    }

    internal void AttemptMove(DungeonTile target)
    {
        List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(myTile, target, movesLeft, IsFlying());
        if(tempPath[tempPath.Count-1]==target&&!target.isOccupied)
        {
            myTile.Vacate(this);
            target.Occupy(this);
            movePath = tempPath;
            movesLeft -= (movePath.Count-1);
            DungeonManager.instance.Wait();
            isAnimating = true;
        }
        else
        {
            foreach(DungeonTile tile in tempPath)
            {
                Debug.Log(gameObject.name+" "+tile.xCoord.ToString() + " " + tile.zCoord.ToString());
            }
            movePath = new List<DungeonTile>();
            movePath.Add(myTile); 
            DungeonManager.instance.Wait();
            isAnimating = true;
        }
    }

    internal void AttemptAttack(Program target)
    {
        if(!hasAttacked&&power>0)
        {
            List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(myTile, target.myTile, range, true);
            if (tempPath[tempPath.Count - 1] == target.myTile)
            {
                Attack newAttack = Instantiate(myAttack, gameObject.transform.position, Quaternion.identity).GetComponent<Attack>();
                newAttack.damage = power;
                newAttack.SetCourse(tempPath,target);
                Program.isTargetingAttack = false;
                if (!keywords.Contains("Hit and Run"))
                {
                    movesLeft = 0;
                }
                hasAttacked = true;
            }
        }
        else
        {
            Debug.Log(gameObject.name + " should not be able to attack");
        }
    }

    internal void AttemptBreach(Hackable toHack)
    {
        if (!hasAttacked && breach > 0)
        {
            int breachRange = 1;
            if(keywords.Contains("Remote"))
            {
                breachRange = range;
            }
            List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(myTile, toHack.myTile, breachRange, true);
            if (tempPath[tempPath.Count - 1] == toHack.myTile)
            {
                Breach newBreach = Instantiate(myBreach, gameObject.transform.position, Quaternion.identity).GetComponent<Breach>();
                newBreach.power = breach;
                newBreach.SetCourse(tempPath, toHack,this);
                Program.isTargetingBreach = false;
                movesLeft = 0;
                hasAttacked = true;
            }
        }
    }

    internal virtual void Damage(int damageAmount)
    {
        size -= damageAmount;
        if(size<=0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        DungeonManager.instance.RemoveProgram(this);
        DungeonManager.instance.UpdateVisibility();
        myTile.Vacate(this);
        Destroy(gameObject);
    }

    internal void BeginPlay()
    {
        myRenderer.enabled = true;
        myIcon.enabled = true;
        GetComponent<Collider>().enabled = true;
        OnStartTurn();
    }
}
