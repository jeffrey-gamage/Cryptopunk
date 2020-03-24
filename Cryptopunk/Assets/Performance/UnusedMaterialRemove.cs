using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnusedMaterialRemove : MonoBehaviour
{
    [SerializeField] float cleanupInterval = 30f;
    private float timeToNextCleanup;
    // Start is called before the first frame update
    void Start()
    {
        timeToNextCleanup = cleanupInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timeToNextCleanup -= Time.deltaTime;
        if(timeToNextCleanup<=0f)
        {
            Resources.UnloadUnusedAssets();
            timeToNextCleanup = cleanupInterval;
        }
    }
}
