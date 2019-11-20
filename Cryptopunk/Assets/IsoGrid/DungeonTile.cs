using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    private static readonly float rampAlignmentDistance = 0.1f;
    private static readonly float flyingHeight = 0.3f;
    private static readonly float rampSlope = -35f;
    private static readonly float unitHeight = 0.7f;
    internal int xCoord;
    internal int zCoord;
    internal bool isBlocked = false;
    [SerializeField] private int height;
    [SerializeField] private bool hasTile = true;
    internal bool isExplored = false;
    internal bool isVisible = false;

    private Collider myCollider;
    private MeshRenderer myMeshRenderer;
    internal Ramp ramp;
    [SerializeField] Material unexplored;
    [SerializeField] Material fog;
    [SerializeField] Material visible;

    internal Program occupyingProgram = null;
    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExplored)
        {
            myMeshRenderer.enabled = true;
            myMeshRenderer.material = unexplored;
        }
        else
        {
            myMeshRenderer.enabled = hasTile;
            if (isVisible)
            {
                myMeshRenderer.material = visible;
            }
            else
            {
                myMeshRenderer.material = fog;
            }

        }
        myCollider.enabled = !isBlocked;
    }

    internal void SetRamp(Ramp newRamp)
    {
        ramp = newRamp;
        ramp.tile = this;
        ramp.transform.position = GetRampCoordinates(ramp.myDirection);
        ramp.transform.Rotate(new Vector3(0f, 90f * (int)ramp.GetDirection(), 0f));
        ramp.transform.Rotate(new Vector3(0f, 0f, rampSlope));
    }

    private Vector3 GetRampCoordinates(Ramp.Direction direction)
    {
        switch(direction)
        {
            case Ramp.Direction.Forward:
                {
                    return (new Vector3(gameObject.transform.position.x,
                                        (float)height * unitHeight / 2f,
                                        gameObject.transform.position.z+rampAlignmentDistance));
                }
            case Ramp.Direction.Right:
                {
                    return (new Vector3(gameObject.transform.position.x + rampAlignmentDistance,
                                        (float)height * unitHeight / 2f,
                                        gameObject.transform.position.z ));
                }
            case Ramp.Direction.Back:
                {
                    return (new Vector3(gameObject.transform.position.x ,
                                        (float)height * unitHeight / 2f,
                                        gameObject.transform.position.z - rampAlignmentDistance));
                }
            default:
                {
                    return (new Vector3(gameObject.transform.position.x - rampAlignmentDistance,
                                        (float)height * unitHeight / 2f,
                                        gameObject.transform.position.z));
                }
        }
    }

    internal void SetHeight(int newHeight)
    {
        this.height = newHeight;
    }

    private void OnMouseDown()
    {
        DungeonManager.instance.SelectTile(this);
    }
    internal int GetHeight()
    {
        return height;
    }
    internal Vector3 GetOccupyingCoordinates(bool isFlying)
        //returns the world coordinates a program should navigate to in order to occupy this tile
    {
        if(isFlying)
        {
            return (new Vector3(gameObject.transform.position.x,
                                (float)height*unitHeight + flyingHeight,
                                gameObject.transform.position.z));
        }
        else
        {
            if(ramp)
            {
                return (new Vector3(gameObject.transform.position.x,
                    ((float)height+0.5f) * unitHeight,
                    gameObject.transform.position.z));
            }
            return (new Vector3(gameObject.transform.position.x,
                                (float)height*unitHeight,
                                gameObject.transform.position.z));
        }
    }
    internal void Reveal()
    {
        if(!isExplored)
        {
            gameObject.transform.position += Vector3.up * height*unitHeight / 2f;
            gameObject.transform.localScale += Vector3.up * height*unitHeight;
            if(ramp)
            {
                ramp.GetComponent<Collider>().enabled = true;
                ramp.GetComponent<MeshRenderer>().enabled = true;
            }
            else if(height<0)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
        isExplored = true;
        isVisible = true;
    }
    internal void Fog()
    {
        isVisible = false;
    }
}
