using System;
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
    public float animationSpeed = 2f;
    public static float unseenMovementSpeed = 12f;
    public static float normalMovementSpeed = 2f;

    private static float rotationRate = 80f;
    private float rotationAmount = 0f;
    private float maxRotation = 35f;

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
        InitializeProgram();
    }

    private void InitializeProgram()
    {
        size = maxSize;
        myIcon = GetComponentInChildren<SpriteRenderer>();
        myRenderer = GetComponent<MeshRenderer>();
        standardMaterial = myRenderer.material;
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        if(myTile&&!myTile.isVisible)
        {
            animationSpeed = unseenMovementSpeed;
        }
        else
        {
            animationSpeed = normalMovementSpeed;
        }
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
            Vector3 motion = GetMotionVector();
            if(!IsFlying())
            {
                HandleRampClimbRotation(motion);
            }
            if (motion == Vector3.zero || motion.magnitude > (myTile.GetOccupyingCoordinates(IsFlying(),false) - gameObject.transform.position).magnitude)
            {
                gameObject.transform.position = myTile.GetOccupyingCoordinates(IsFlying(),false);
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

    private void HandleRampClimbRotation(Vector3 motion)
    {
        Vector3 rotationAxis = GetRotationAxis(motion);
        if (myTile.ramp && rotationAmount<maxRotation)
        {
            rotationAmount = Mathf.Clamp(rotationAmount+rotationRate * Time.deltaTime,0f,maxRotation);
        }
        else if(!myTile.ramp&&rotationAmount>0f)
        {
            rotationAmount = Mathf.Clamp(rotationAmount - rotationRate * Time.deltaTime, 0f, maxRotation);
        }
        gameObject.transform.rotation = Quaternion.AngleAxis(rotationAmount, rotationAxis);
    }

    private Vector3 GetRotationAxis(Vector3 motion)
    {
        if(motion.x>0)//moving right
        {
            if(motion.y<0)
            {
                return Vector3.back;
            }
            else
            {
                return Vector3.forward;
            }
        }
        if(motion.x<0)//moving left
        {
            if(motion.y<0)
            {
                return Vector3.forward;
            }
            else
            {
                return Vector3.back;
            }
        }
        if(motion.z>0)//moving forward
        {
            if(motion.y>0)
            {
                return Vector3.left;
            }
            else
            {
                return Vector3.right;
            }
        }
        else//moving backward
        {
            if(motion.y>0)
            {
                return Vector3.right;
            }
            else
            {
                return Vector3.left;
            }
        }
    }

    private Vector3 GetMotionVector()
    {
        Vector3 motionVector = myTile.GetOccupyingCoordinates(IsFlying(),false) - gameObject.transform.position;
        if (IsFlying())
        {
            if (motionVector.y < 0 && (motionVector - Vector3.up * motionVector.y).magnitude > animationSpeed * Time.deltaTime)//don't descend until hovering over destination
            {
                motionVector -= Vector3.up * motionVector.y;
            }
            else if(motionVector.y>animationSpeed*Time.deltaTime)//ascend before moving laterally
            {
                motionVector = new Vector3(0, motionVector.y, 0);
            }
        }
        motionVector = motionVector.normalized * animationSpeed * Time.deltaTime;

        return motionVector;
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
        if(keywords.Contains("Recover"))
        {
            size = Math.Min(maxSize, size + 1);
        }
    }

    internal virtual void OnMouseOver()
    {
        if (isTargetingAttack && !DungeonManager.instance.IsPlayers(this) && Program.selectedProgram.IsControlledByPlayer())
        {
            DungeonManager.instance.PreviewTile(this.myTile);
        }
        if (Input.GetMouseButtonDown(0))//select
        {
            if (DungeonManager.instance.mode != DungeonManager.Mode.Deploy || !hasBegunPlay)//prevent port deployment from moving your programs
            {
                isTargetingAttack = false;
                isTargetingBreach = false;
                DungeonManager.instance.mode = DungeonManager.Mode.Move;
                selectedProgram = this;
                FindObjectOfType<PathPreview>().ClearPreview();
                Hackable.selectedObject = GetComponent<Hackable>();
                ClearSightPreviews();
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if (isTargetingAttack && !DungeonManager.instance.IsPlayers(this) && Program.selectedProgram.IsControlledByPlayer())
            {
                Program.selectedProgram.AttemptAttack(this);
            }
        }
    }

    private void ClearSightPreviews()
    {
        foreach(EnemyProgram enemyProgram in DungeonManager.instance.GetAICotrolledPrograms())
        {
            enemyProgram.ClearSightPreview();
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
                newBreach.breach = breach;
                newBreach.SetCourse(tempPath, toHack);
                Program.isTargetingBreach = false;
                movesLeft = 0;
                hasAttacked = true;
            }
        }
    }

    internal virtual void Damage(int damageAmount)
    {
        if(keywords.Contains("armored")&&damageAmount>0)
        {
            damageAmount--;
        }
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
