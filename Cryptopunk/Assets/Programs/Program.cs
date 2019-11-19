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
    [SerializeField] internal string[] keywords;
    [SerializeField]internal DungeonTile myTile;
    internal int size;
    internal int movesLeft;
    internal bool hasAttacked;
    private List<DungeonTile> movePath;
    // Start is called before the first frame update
    void Start()
    {
        size = maxSize;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateMovement();
    }

    private void AnimateMovement()
    {
        if (isAnimating)
        {
            Vector3 motion = (myTile.transform.position - gameObject.transform.position).normalized * animationSpeed * Time.deltaTime;
            if (motion==Vector3.zero||motion.magnitude > (myTile.transform.position - gameObject.transform.position).magnitude)
            {
                gameObject.transform.position = myTile.transform.position;
                if (movePath.Count == 0)
                {
                    isAnimating = false;
                    DungeonManager.instance.Resume();
                }
                else
                {
                    myTile = movePath[0];
                    movePath.Remove(myTile);
                }
            }
            else
            {
                gameObject.transform.position += motion;
            }
        }
    }

    internal virtual void OnStartTurn()
    {
        hasAttacked = false;
        movesLeft = speed;
    }

    private void OnMouseDown()
    {
        if (isTargeting)
        {
            target = this;
        }
        else
        {
            selectedProgram = this;
        }
    }

    internal void AttemptMove(DungeonTile target, DungeonGrid grid)
    {
        List<DungeonTile> tempPath = grid.FindPath(myTile, target, movesLeft, false);
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
}
