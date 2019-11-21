using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public bool isAnimating = false;
    public static bool isTargeting = false;
    public static float animationSpeed = 2f;
    public static Program selectedProgram;
    public static Program target;
    [SerializeField] internal int maxSize;
    [SerializeField] internal int power;
    [SerializeField] internal int speed;
    [SerializeField] internal int range;
    [SerializeField] internal int sight;
    [SerializeField] internal int breach;
    [SerializeField] internal List<string> keywords;
    [SerializeField] GameObject myAttack;
    internal DungeonTile myTile;
    internal int size;
    internal int movesLeft;
    internal bool hasAttacked;
    private List<DungeonTile> movePath;
    // Start is called before the first frame update
    internal virtual void Start()
    {
        size = maxSize;
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        AnimateMovement();
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
            }
            else
            {
                gameObject.transform.position += motion;
            }
        }
    }

    internal bool IsFlying()
    {
        return keywords.Contains("Flying");
    }

    internal virtual void OnStartTurn()
    {
        hasAttacked = false;
        movesLeft = speed;
    }

    private void OnMouseDown()
    {
        if (isTargeting&&!DungeonManager.instance.IsPlayers(this))
        {
            Program.selectedProgram.AttemptAttack(this);
        }
        else
        {
            selectedProgram = this;
        }
    }

    internal void AttemptMove(DungeonTile target)
    {
        List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(myTile, target, movesLeft, false);
        if(tempPath[tempPath.Count-1]==target)
        {
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
                Program.isTargeting = false;
                movesLeft = 0;//TODO: Make exception for hit and run programs
                hasAttacked = true;
            }
        }
        else
        {
            Debug.Log(gameObject.name + " should not be able to attack");
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

    private void Die()
    {
        DungeonManager.instance.RemoveProgram(this);
        DungeonManager.instance.UpdateVisibility();
        Destroy(gameObject);
    }
}
