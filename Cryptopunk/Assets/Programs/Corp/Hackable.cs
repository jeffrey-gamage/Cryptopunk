using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    internal MeshRenderer myMeshRenderer;
    public static Hackable selectedObject;
    [SerializeField] internal int maxIntegrity;
    [SerializeField] int rebootTime = 3;
    internal int currentIntegrity;
    internal DungeonTile myTile;
    private bool isHacked;
    internal bool isEnabled = true;
    internal Program myProgram;
    internal int rebootCountdown=0;
    private Material normalMaterial;
    [SerializeField] Material hackedMaterial;

    internal virtual void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = myMeshRenderer.material;
        myProgram = GetComponent<Program>();
        Reboot();
    }
    internal virtual void Update()
    {
        if (!myProgram)
        {
            if (myTile.isExplored&&myTile.IsFinishedRevealAnimation())
            {
                myMeshRenderer.enabled = true;
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.enabled = true;
                }
            }
            else
            {
                myMeshRenderer.enabled = false;
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.enabled = false;
                }
            }
        }
    }

    internal void OnStartTurn()
    {
        if(isHacked)
        {
            rebootCountdown--;
            if(rebootCountdown<=0)
            {
                Reboot();
            }
        }
    }

    private void Reboot()
    {
        myMeshRenderer.material = normalMaterial;
        rebootCountdown = rebootTime;
        currentIntegrity = maxIntegrity;
        isHacked = false;
        Activate();
    }

    protected virtual void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (DungeonManager.instance.mode != DungeonManager.Mode.Deploy)
            {
                Program.selectedProgram = myProgram;
                FindObjectOfType<PathPreview>().ClearPreview();
                selectedObject = this;
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if (Program.isTargetingBreach && Program.selectedProgram && Program.selectedProgram.IsControlledByPlayer())
            {
                Program.selectedProgram.AttemptBreach(this);
            }
            else if((!Program.isTargetingAttack)&&(!Program.isTargetingBreach)&&Program.selectedProgram&&Program.selectedProgram.IsControlledByPlayer())
            {
                DungeonManager.instance.RightClickTile(myTile);
            }
        }
    }
    internal void Disrupt(int damageAmount)
    {
        currentIntegrity = Mathf.Clamp(currentIntegrity - damageAmount, 1, maxIntegrity);
    }

    internal virtual void Breach(int breachAmount)
    {
        currentIntegrity= Mathf.Clamp(currentIntegrity - breachAmount, 0, maxIntegrity);
        if(currentIntegrity<=0)
        {
            rebootCountdown = rebootTime;
            if (GetComponent<EnemyProgram>())
            {
                GetComponent<EnemyProgram>().OnStartTurn();
                GetComponent<EnemyProgram>().ClearSightPreview();
                rebootCountdown++;
                currentIntegrity = 0;
            }
            this.isHacked = true;
            myMeshRenderer.material = hackedMaterial;
            Deactivate();
        }
    }
    internal void SetTile(DungeonTile tile)
    {
        myTile = tile;
        if (isEnabled)
        {
            tile.isBlocked = true;
        }
    }
    internal virtual void Activate()
    {
        //hook for subclasses to do behaviour when switched on
        isEnabled = true;
        myTile.isBlocked = true;
    }
    internal virtual void Deactivate()
    {
        //hook for subclasses to do behaviour when switched off
        isEnabled = false;
    }

    internal bool IsHacked()
    {
        return isHacked;
    }
}
