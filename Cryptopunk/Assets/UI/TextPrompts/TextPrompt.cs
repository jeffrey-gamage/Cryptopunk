using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPrompt : MonoBehaviour
{
    [SerializeField] internal Vector3Int[] TriggerPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Accept()
    {
        Destroy(gameObject);
    }
}
