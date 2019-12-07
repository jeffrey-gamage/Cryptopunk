using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : Hackable
{
    
    [SerializeField] float inactiveHeight = 0.1f;
    private Vector3 inactiveScale;
    private Vector3 activeScale;
    // Start is called before the first frame update
    internal override void Start()
    {
        activeScale = gameObject.transform.localScale;
        inactiveScale = new Vector3(activeScale.x, inactiveHeight, activeScale.z);
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        base.Update();
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
    internal override void Activate()
    {
        if(!myTile.isOccupied)
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
