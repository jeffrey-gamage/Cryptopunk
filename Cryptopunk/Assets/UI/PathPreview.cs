using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public static PathPreview instance;
    public List<SpriteRenderer> pathTrace;
    private Color pathColor;
    [SerializeField] float pathPreviewOffset = 0.2f;
    [SerializeField] float pathAlpha = 0.3f;
    [SerializeField] GameObject pathSegment;
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
        pathColor = new Color(previewColor.r,previewColor.g,previewColor.b,pathAlpha);
    }

    internal void DisplayPreview(List<DungeonTile> previewPath)
    {

        if (previewPath.Count > 1&&!(DungeonManager.instance.mode == DungeonManager.Mode.Wait))
        {
            for (int i = 0; i < pathTrace.Count; i++)
            {
                Destroy(pathTrace[i].gameObject);
            }
            pathTrace = new List<SpriteRenderer>();
            for (int i = 1; i < previewPath.Count; i++)
            {
                pathTrace.Add(Instantiate(pathSegment, previewPath[i].GetOccupyingCoordinates(false)+Vector3.up*pathPreviewOffset, previewPath[i].getOccupantRotation()).GetComponent<SpriteRenderer>());
            }
        }
        else
        {
            for (int i = pathTrace.Count - 1; i >= 0; i--)
            {
                if (i >= 0)
                {
                    GameObject temp = pathTrace[i].gameObject;
                    pathTrace.RemoveAt(i);
                    Destroy(temp);
                }
            }
        }
    }
}
