using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    private static float flyingHeight = 0.3f;
    internal int xCoord;
    internal int zCoord;
    internal bool isBlocked = false;
    [SerializeField] private int height;
    [SerializeField] private bool hasTile = true;
    internal bool isExplored = false;
    internal bool isVisible = false;

    private Collider myCollider;
    private MeshRenderer myMeshRenderer;
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
                                (float)height + flyingHeight,
                                gameObject.transform.position.z));
        }
        else
        {

            return (new Vector3(gameObject.transform.position.x,
                                (float)height,
                                gameObject.transform.position.z));
        }
    }
    internal void Reveal()
    {
        if(!isExplored)
        {
            gameObject.transform.position += Vector3.up * height / 2f;
            gameObject.transform.localScale += Vector3.up * height;
        }
        isExplored = true;
        isVisible = true;
    }
    internal void Fog()
    {
        isVisible = false;
    }
}
