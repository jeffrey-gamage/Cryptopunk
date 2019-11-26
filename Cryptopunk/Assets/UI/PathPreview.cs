using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public static PathPreview instance;
    public List<SpriteRenderer> pathTrace;
    private Color pathColor;
    [SerializeField] Vector3 pathPreviewOffset = new Vector3(0f, 0.2f, 0f);
    [SerializeField] Sprite endpoint;
    [SerializeField] Sprite straight;
    [SerializeField] Sprite corner;
    [SerializeField] GameObject pathPoint;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SpriteRenderer renderer in pathTrace)
        {
            renderer.color = pathColor;
        }
    }

    internal void SetColor(Color previewColor)
    {
        pathColor = previewColor;
    }

    internal void DisplayPreview(List<DungeonTile> previewPath)
    {
        for(int i=0;i<pathTrace.Count;i++)
        {
            Destroy(pathTrace[i].gameObject);
        }
        pathTrace = new List<SpriteRenderer>();
        for(int i=0;i<previewPath.Count;i++)
        {
            pathTrace.Add(Instantiate(pathPoint, previewPath[i].GetOccupyingCoordinates(false)+ pathPreviewOffset, Quaternion.identity).GetComponent<SpriteRenderer>());
            if(i==0||i==previewPath.Count-1)
            {
                //use endpoint sprite
            }
            else
            {
                pathTrace[i].sprite = straight;
            }
        }
    }
}
