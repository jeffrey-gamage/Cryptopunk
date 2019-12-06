using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : Hackable
{
    MeshRenderer myMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        Activate();
    }

    // Update is called once per frame
    void Update()
    {
        myMeshRenderer.enabled = isEnabled && myTile.isExplored;
    }
    internal override void Activate()
    {
        if(myTile.isOccupied)
        {
            Destroy(gameObject);
        }
        else
        {
            base.Activate();
            myTile.isBlocked = true;
        }
    }
    internal override void Deactivate()
    {
        base.Deactivate();
        myTile.isBlocked = false;
    }

}
