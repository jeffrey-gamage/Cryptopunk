using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    static float lifeTime = 0.75f;
    private float lifeCounter;
    // Start is called before the first frame update
    void Start()
    {
        lifeCounter = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        lifeCounter -= Time.deltaTime;
        if(lifeCounter<=0f)
        {
            Destroy(gameObject);
        }
    }
}
