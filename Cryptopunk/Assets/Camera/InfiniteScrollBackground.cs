using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScrollBackground : MonoBehaviour
{
    [SerializeField] GameObject leftBackground;
    [SerializeField] GameObject rightBackground;
    [SerializeField] float scrollRate = 0.2f;
    private float length;
    private float scrollValue = 0f;

    Vector3 leftHomePosition;
    Vector3 rightHomePosition;
    // Start is called before the first frame update
    void Start()
    {
        length = leftBackground.GetComponent<SpriteRenderer>().bounds.size.x;
        leftHomePosition = leftBackground.transform.localPosition;
        rightHomePosition = rightBackground.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        leftBackground.transform.localPosition = leftHomePosition + Vector3.right * length * scrollValue;
        rightBackground.transform.localPosition = rightHomePosition + Vector3.right * length * scrollValue;
        if(scrollValue>0.3f)
        {
            GameObject temp = leftBackground;
            leftBackground = rightBackground;
            rightBackground = temp;
            scrollValue -= 0.5f;
        }
        else if(scrollValue<-0.3f)
        {
            GameObject temp = rightBackground;
            rightBackground = leftBackground;
            leftBackground = temp;
            scrollValue += 0.5f;
        }
    }

    internal void Scroll(float rawScrollDelta)
    {
        scrollValue += rawScrollDelta * scrollRate;
    }
}
