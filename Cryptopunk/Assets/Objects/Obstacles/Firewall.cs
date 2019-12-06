using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : Hackable
{
    MeshRenderer myMeshRenderer;
    [SerializeField] float inactiveHeight = 0.1f;
    private Vector3 inactiveScale;
    private Vector3 activeScale;
    // Start is called before the first frame update
    internal override void Start()
    {
        activeScale = gameObject.transform.localScale;
        inactiveScale = new Vector3(activeScale.x, inactiveHeight, activeScale.z);
        myMeshRenderer = GetComponent<MeshRenderer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(myTile.isExplored)
        {
            myMeshRenderer.enabled = true;
            if (isEnabled)
            {
                gameObject.transform.localScale = activeScale;
                gameObject.transform.position = myTile.GetOccupyingCoordinates(true);
            }
            else
            {
                gameObject.transform.localScale = inactiveScale;
                gameObject.transform.position = myTile.GetOccupyingCoordinates(false);
            }
        }
        else
        {
            myMeshRenderer.enabled = false;
        }
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
