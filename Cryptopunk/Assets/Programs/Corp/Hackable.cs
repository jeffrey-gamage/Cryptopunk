using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    internal MeshRenderer myMeshRenderer;
    internal Collider myCollider;
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
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material inactiveHackedMaterial;

    internal virtual void Start()
    {
        myCollider = GetComponent<Collider>();
        myMeshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = myMeshRenderer.material;
        myProgram = GetComponent<Program>();
        Reboot();
    }
    internal virtual void Update()
    {
        if (!myProgram)
        {
            bool objectIsVisible = myTile.IsExplored() && myTile.IsFinishedRevealAnimation();
            myCollider.enabled = objectIsVisible;
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = objectIsVisible;
            }
        }
    }

    internal void ApplyAppropriateMaterial()
    {
        if(isHacked)
        {
            if(isEnabled&&hackedMaterial)
            {
                myMeshRenderer.material = hackedMaterial;
            }
            else if(inactiveHackedMaterial)
            {
                myMeshRenderer.material = inactiveHackedMaterial;
            }
        }
        else
        {
            if (isEnabled)
            {
                myMeshRenderer.material = normalMaterial;
            }
            else if (inactiveMaterial)
            {
                myMeshRenderer.material = inactiveMaterial;
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
        if(DungeonManager.instance.mode!=DungeonManager.Mode.Move||(Program.selectedProgram&&Program.selectedProgram.IsFlying())||!myTile.isBlocked)
        {
            myTile.OnMouseOver();
        }
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
            Deactivate(true);
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
        ApplyAppropriateMaterial();
    }
    internal virtual void Deactivate(bool isBreach)
    {
        //hook for subclasses to do behaviour when switched off
        if((!isBreach)||!GetComponent<EnemyProgram>())
        {
            isEnabled = false;
        }
        ApplyAppropriateMaterial();
    }

    internal bool IsHacked()
    {
        return isHacked;
    }
}
