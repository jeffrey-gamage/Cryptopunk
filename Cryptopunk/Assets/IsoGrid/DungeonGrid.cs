using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField] GameObject chasm;
    [SerializeField] GameObject grid0;
    [SerializeField] GameObject grid1;
    [SerializeField] GameObject grid2;
    [SerializeField] GameObject grid3;
    private DungeonTile[][] tileGrid;
    [SerializeField] int numSegments = 3;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void GenerateGrid(int[][] gridHeights)
    {
        tileGrid = new DungeonTile[gridHeights.Length][];
        for(int i=0; i<gridHeights.Length;i++)
        {
            tileGrid[i] = new DungeonTile[gridHeights[i].Length];
            for (int j = 0; j < gridHeights[i].Length; j++)
            {
                switch(gridHeights[i][j])
                {
                    case 0:
                        {
                            tileGrid[i][j] = Instantiate(grid0, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 1:
                        {
                            tileGrid[i][j] = Instantiate(grid1, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 2:
                        {
                            tileGrid[i][j] = Instantiate(grid2, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 3:
                        {
                            tileGrid[i][j] = Instantiate(grid3, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    default:
                        {
                            tileGrid[i][j] = Instantiate(chasm, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                }
                tileGrid[i][j].xCoord = i;
                tileGrid[i][j].zCoord = j;
            }
        }
    }

    internal List<DungeonTile> FindPath(DungeonTile start, DungeonTile end, int pathLength, bool isFlyingPath)
    {
        List<DungeonTile> path = new List<DungeonTile>();
        path.Add(start);
        while(path.Count<=pathLength&&(path.Count==0||path[path.Count-1]!=end))
        {
            if(end.xCoord>path[path.Count-1].xCoord&&IsValidPath(path[path.Count - 1].xCoord + 1,path[path.Count - 1].zCoord))
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord + 1][path[path.Count - 1].zCoord]);
            }
            else if(end.xCoord < path[path.Count - 1].xCoord && IsValidPath(path[path.Count - 1].xCoord - 1, path[path.Count - 1].zCoord))
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord - 1][path[path.Count - 1].zCoord]);
            }
            if (end.zCoord > path[path.Count - 1].zCoord && IsValidPath(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord + 1))
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord + 1]);
            }
            else if (end.zCoord < path[path.Count - 1].zCoord && IsValidPath(path[path.Count - 1].xCoord, path[path.Count - 1].zCoord - 1))
            {
                path.Add(tileGrid[path[path.Count - 1].xCoord][path[path.Count - 1].zCoord - 1]);
            }
        }
        return path;
    }

    private bool IsValidPath(int xCoord, int zCoord)
    {
        if(xCoord<0||xCoord>=tileGrid.Length)
        {
            return false;
        }
        if(zCoord<0||zCoord>=tileGrid.Length)
        {
            return false;
        }
        return true;
    }
}
