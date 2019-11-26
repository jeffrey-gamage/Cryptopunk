using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public static PathPreview instance;
    public List<DungeonTile> previewPath;
    public List<SpriteRenderer> pathTrace;
    private Color pathColor;
    // Start is called before the first frame update
    void Start()
    {
        previewPath = new List<DungeonTile>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            foreach(DungeonTile tile in previewPath)
            {
                Debug.Log(tile.xCoord.ToString() + ", " + tile.zCoord.ToString());
            }
        }
    }

    internal void SetColor(Color previewColor)
    {
        pathColor = previewColor;
    }
}
