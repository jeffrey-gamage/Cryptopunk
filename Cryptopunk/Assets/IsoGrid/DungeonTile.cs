using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    private int xCoord;
    private int zCoord;
    private bool isBlocked = false;
    [SerializeField] private int height;
    [SerializeField] private bool hasTile = true;
    internal bool isExplored = false;
    internal bool isVisible = false;

    private MeshRenderer myMeshRenderer;
    [SerializeField] Material unexplored;
    [SerializeField] Material fog;
    [SerializeField] Material visible;
    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isExplored)
        {
            myMeshRenderer.enabled = true;
            myMeshRenderer.material = unexplored;
        }
        else
        {
            myMeshRenderer.enabled = hasTile;
            if(visible)
            {
                myMeshRenderer.material = visible;
            }
            else
            {
                myMeshRenderer.material = fog;
            }
           
        }
    }
    internal int GetHeight()
    {
        return height;
    }
}
