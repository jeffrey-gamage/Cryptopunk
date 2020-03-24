using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentZone : MonoBehaviour
{
    public static DeploymentZone instance;
    public Vector3Int myCoords;
    public int range = 1;
    // Start is called before the first frame update
    void Start()
    {
        if(instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
