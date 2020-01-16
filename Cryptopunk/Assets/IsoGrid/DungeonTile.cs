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
    internal bool isOccupied = false;
    [SerializeField] private int height;
    internal bool isExplored = false;
    internal bool isVisible = false;
    
    private MeshRenderer myMeshRenderer;
    internal Loot loot;
    internal Collider myCollider;
    internal Ramp ramp;
    [SerializeField] Material unexplored;
    [SerializeField] Material fog;
    [SerializeField] Material visible;
    
    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        if (myCollider == null)
        {
            myCollider = GetComponent<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        myCollider.enabled = isExplored;
        if (!isExplored)
        {
            myMeshRenderer.enabled = true;
            myMeshRenderer.material = unexplored;
        }
        else
        {
            if (loot)
            {
                loot.GetComponent<MeshRenderer>().enabled = true;
            }
            myMeshRenderer.enabled = height>=0;
            if (isVisible)
            {
                myMeshRenderer.material = visible;
            }
            else
            {
                myMeshRenderer.material = fog;
            }
        }
        if (ramp)
        {
            ramp.myRenderer.material = myMeshRenderer.material;
        }
    }

    internal Quaternion getOccupantRotation()
    {
        Quaternion occupantRotation = Quaternion.identity;
        if(ramp)
        {
            occupantRotation*= ramp.transform.rotation;
        }
        occupantRotation *= Quaternion.AngleAxis(90f, Vector3.right);
        return occupantRotation;
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
                                        (float)height * unitHeight,
                                        gameObject.transform.position.z+rampAlignmentDistance));
                }
            case Ramp.Direction.Right:
                {
                    return (new Vector3(gameObject.transform.position.x + rampAlignmentDistance,
                                        (float)height * unitHeight,
                                        gameObject.transform.position.z ));
                }
            case Ramp.Direction.Back:
                {
                    return (new Vector3(gameObject.transform.position.x ,
                                        (float)height * unitHeight,
                                        gameObject.transform.position.z - rampAlignmentDistance));
                }
            default:
                {
                    return (new Vector3(gameObject.transform.position.x - rampAlignmentDistance,
                                        (float)height * unitHeight,
                                        gameObject.transform.position.z));
                }
        }
    }

    internal void Occupy(Program program)
    {
        isBlocked = !program.IsFlying();
        isOccupied = true;
    }

    internal void Vacate(Program program)
    {
        isBlocked = false;
        isOccupied = false;
    }

    internal void SetHeight(int newHeight)
    {
        this.height = newHeight;
    }

    private void OnMouseDown()
    {
        DungeonManager.instance.SelectTile(this);
    }
    private void OnMouseOver()
    {
        if (DungeonManager.instance.mode == DungeonManager.Mode.Move)
        {
            DungeonManager.instance.PreviewTile(this);
        }
    }
    internal int GetHeight()
    {
        return height;
    }
    internal Vector3 GetOccupyingCoordinates(bool isFlying)
        //returns the world coordinates a program should navigate to in order to occupy this tile
    {
        Vector3 occupyCoordinates = new Vector3(gameObject.transform.position.x,
                                Mathf.Max((float)height * unitHeight,0f),
                                gameObject.transform.position.z);
        if (isFlying)
        {
            occupyCoordinates += Vector3.up * flyingHeight;
        }
        if(ramp)
        {
            occupyCoordinates += Vector3.up * 0.5f*unitHeight;
        }
        return occupyCoordinates;
}
    internal void Reveal()
    {
        if(!isExplored)
        {
            gameObject.transform.position += Vector3.up * height*unitHeight / 2f;
            if (height >= 0)
            {
                gameObject.transform.localScale += Vector3.up * height * unitHeight;
            }
            if(ramp)
            {
                ramp.GetComponent<Collider>().enabled = true;
                ramp.GetComponent<MeshRenderer>().enabled = true;
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
