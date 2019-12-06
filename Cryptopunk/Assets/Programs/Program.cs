using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    protected MeshRenderer myRenderer;
    private SpriteRenderer myIcon;
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
    internal int size;
    internal int movesLeft;
    internal bool hasAttacked;
    internal bool hasBeenSpotted;
    internal List<DungeonTile> movePath;

    private Material standardMaterial;
    [SerializeField] Material stealthMaterial;
    [SerializeField] float iconStealthAlpha = 0.3f;
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
        AnimateMovement();
        ShowStealthVisuals();
    }

    private void ShowStealthVisuals()
    {
        if (myIcon)
        {
            myIcon.enabled = myRenderer.enabled;
            if (IsStealthed())
            {
                myIcon.color = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, iconStealthAlpha);
            }
            else
            {
                myIcon.color = new Color(myIcon.color.r, myIcon.color.g, myIcon.color.b, 1);
            }
        }
        if (IsStealthed())
        {
            myRenderer.material = stealthMaterial;
        }
        else
        {
            myRenderer.material = standardMaterial;
        }
    }

    private void AnimateMovement()
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
        else
        {
            selectedProgram = this;
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
            Debug.Log("Found path, animating movement");
            movePath = tempPath;
            movesLeft -= (movePath.Count-1);
            DungeonManager.instance.Wait();
            isAnimating = true;
        }
        else
        {
            Debug.Log("no path found.");
            foreach(DungeonTile tile in tempPath)
            {
                Debug.Log(tile.xCoord.ToString() + " " + tile.zCoord.ToString());
            }
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
                movesLeft = 0;//TODO: Make exception for hit and run programs
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
        Destroy(gameObject);
    }
}
