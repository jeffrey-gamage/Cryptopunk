using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public static bool isAnimating = false;
    public static float animationSpeed = 2f;
    public static Program selectedProgram;
    [SerializeField] internal int size;
    [SerializeField] internal int power;
    [SerializeField] internal int speed;
    [SerializeField] internal int range;
    [SerializeField] internal int sight;
    [SerializeField] internal int breach;
    [SerializeField] internal string[] keywords;
    internal DungeonTile myTile;
    private int movesLeft;
    private bool hasAttacked;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
