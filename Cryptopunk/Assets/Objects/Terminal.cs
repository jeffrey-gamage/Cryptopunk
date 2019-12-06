using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Hackable
{
    internal List<Hackable> controlledObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    internal override void Activate()
    {
        foreach (Hackable obj in controlledObjects)
        {
            obj.Activate();
        }
    }
    internal override void Deactivate()
    {
        foreach(Hackable obj in controlledObjects)
        {
            obj.Deactivate();
        }
    }

}
