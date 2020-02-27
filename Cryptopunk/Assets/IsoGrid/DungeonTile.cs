﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonTile : MonoBehaviour
{
    private static readonly float baseDropDistance = 5f;
    private static readonly float dropDistanceRange = 2.5f;
    private static readonly float maxRevealDelayTime=0.75f;
    private static readonly float animationSpeed = 5f;
    private Vector3 homeLocation;

    private static readonly float rampAlignmentDistance = 0.1f;
    private static readonly float flyingHeight = 0.3f;
    private static readonly float rampSlope = -35f;
    private static readonly float unitHeight = 0.7f;
    private static readonly float baseHeight = 0.125f;
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
    private float revealAnimationCountDown = -99;

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
        HandleRevealAnimation();
        myCollider.enabled = isExplored;
        if (!isExplored)
        {
            myMeshRenderer.enabled = revealAnimationCountDown<=0f;
            myMeshRenderer.material = unexplored;
        }
        else
        {
            if (loot)
            {
                loot.GetComponent<MeshRenderer>().enabled = true;
            }
            myMeshRenderer.enabled = height >= 0;
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

    private void HandleRevealAnimation()
    {
        if (revealAnimationCountDown > 0)
        {
            revealAnimationCountDown -= Time.deltaTime;
        }
        else if (revealAnimationCountDown > -50)
        {
            Vector3 motion = homeLocation - gameObject.transform.position;
            if (animationSpeed * Time.deltaTime > motion.magnitude)
            {
                revealAnimationCountDown = -99;
            }
            else
            {
                motion = motion.normalized * animationSpeed * Time.deltaTime;
            }
            gameObject.transform.position += motion;
        }
    }

    internal Quaternion getPreviewRotation()
    {
        Quaternion previewRotation = Quaternion.identity;
        if(ramp)
        {
            previewRotation*= ramp.transform.rotation;
        }
        previewRotation *= Quaternion.AngleAxis(90f, Vector3.right);
        return previewRotation;
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
        occupyCoordinates += Vector3.up*baseHeight;
        return occupyCoordinates;
}
    internal void Reveal()
    {
        if(!isExplored)
        {
            homeLocation = gameObject.transform.position + Vector3.up * height*unitHeight / 2f;
            if (!myMeshRenderer)
            {
                myMeshRenderer = GetComponent<MeshRenderer>();
            }
            myMeshRenderer.enabled = false;
            gameObject.transform.position -= Vector3.up * Random.Range(baseDropDistance, baseDropDistance + dropDistanceRange);
            revealAnimationCountDown = Random.Range(0f, maxRevealDelayTime);
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
