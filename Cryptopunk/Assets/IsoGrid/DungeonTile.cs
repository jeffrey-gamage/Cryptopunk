using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    internal int xCoord;
    internal int zCoord;
    private bool isBlocked = false;
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
    private void OnMouseDown()
    {
        DungeonManager.instance.SelectTile(this);
    }
    internal int GetHeight()
    {
        return height;
    }
    internal void Reveal()
    {
        isExplored = true;
        isVisible = true;
    }
    internal void Fog()
    {
        isVisible = false;
    }
}
