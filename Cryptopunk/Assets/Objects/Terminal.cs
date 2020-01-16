using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Hackable
{
    [SerializeField] GameObject linkStart;
    [SerializeField] GameObject linkLR;
    [SerializeField] GameObject linkFB;
    [SerializeField] GameObject linkVert;
    [SerializeField] GameObject linkEnd;
    [SerializeField] float linkOffset = 1.5f;
    public List<Hackable> controlledObjects;

    internal override void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    internal override void Update()
    {
        base.Update();
        if (controlledObjects.Count > 0&&controlledObjects[0])
        {
            linkStart.transform.position = gameObject.transform.position + Vector3.up * linkOffset;
            linkEnd.transform.position = controlledObjects[0].transform.position + Vector3.up * linkOffset;
            linkLR.transform.position = new Vector3((controlledObjects[0].transform.position.x + gameObject.transform.position.x) / 2f, gameObject.transform.position.y+linkOffset, gameObject.transform.position.z);
            linkLR.transform.localScale = new Vector3(Mathf.Abs(controlledObjects[0].transform.position.x - gameObject.transform.position.x), 1, 1)*1.25f;
            linkVert.transform.position = new Vector3(controlledObjects[0].transform.position.x, (controlledObjects[0].transform.position.y + gameObject.transform.position.y) / 2f+linkOffset, gameObject.transform.position.z);
            linkVert.transform.localScale = new Vector3(Mathf.Abs(controlledObjects[0].transform.position.y - gameObject.transform.position.y), 1, 1) * 2.5f;
            linkFB.transform.position = new Vector3(controlledObjects[0].transform.position.x, controlledObjects[0].transform.position.y+linkOffset, (controlledObjects[0].transform.position.z + gameObject.transform.position.z) / 2f);
            linkFB.transform.localScale = new Vector3(Mathf.Abs(controlledObjects[0].transform.position.z - gameObject.transform.position.z), 1, 1)*1.25f;
        }
        else
        {
            if(linkStart)
            {
                Destroy(linkStart);
                Destroy(linkEnd);
                Destroy(linkLR);
                Destroy(linkVert);
                Destroy(linkFB);
            }
        }
    }
    internal override void Activate()
    {
        base.Activate();
        foreach (Hackable obj in controlledObjects)
        {
            obj.Activate();
        }
    }
    internal override void Deactivate()
    {
        base.Deactivate();
        foreach(Hackable obj in controlledObjects)
        {
            obj.Deactivate();
        }
    }

}
