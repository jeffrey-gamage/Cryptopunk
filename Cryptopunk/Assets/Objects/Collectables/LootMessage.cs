using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootMessage : MonoBehaviour
{
    [SerializeField] TextMesh pickupText;
    [SerializeField] float climbSpeed = 2.5f;
    [SerializeField] float lifeTime = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if(lifeTime<=0f)
        {
            Destroy(gameObject);
        }
        else
        {
            lifeTime -= Time.deltaTime;
            gameObject.transform.position += Vector3.up * climbSpeed * Time.deltaTime;
        }
    }

    internal void SetText(string text)
    {
        pickupText.text = text;
    }
}
