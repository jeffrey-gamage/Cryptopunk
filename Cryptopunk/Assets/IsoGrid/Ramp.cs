using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    internal enum Direction
    {
        Left,Forward, Right,Back
    }
    internal Direction myDirection;
    internal DungeonTile tile;
    internal MeshRenderer myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
    }
    
    internal void SetDirection(Direction direction)
    {
        myDirection = direction;
    }
    internal Direction GetDirection()
    {
        return myDirection;
    }
    private void OnMouseDown()
    {
        if(tile)
        {
            DungeonManager.instance.SelectTile(tile);
        }
        else
        {
            Debug.Log(name + " missing tile");
        }
    }
}
