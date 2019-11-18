using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public static bool isAnimating = false;
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
            if (motion.magnitude > (myTile.transform.position - gameObject.transform.position).magnitude)
            {
                gameObject.transform.position = myTile.transform.position;
                isAnimating = false;
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

    public void Move(DungeonTile destination)
    {
        myTile = destination;
        isAnimating = true;
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
        movePath = grid.FindPath(myTile, target, movesLeft, false);
    }
}
