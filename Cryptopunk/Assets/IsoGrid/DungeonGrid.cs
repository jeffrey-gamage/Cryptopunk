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
    private DungeonTile[][] grid;
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
        grid = new DungeonTile[gridHeights.Length][];
        for(int i=0; i<gridHeights.Length;i++)
        {
            grid[i] = new DungeonTile[gridHeights[i].Length];
            for (int j = 0; j < gridHeights[i].Length; j++)
            {
                switch(gridHeights[i][j])
                {
                    case 0:
                        {
                            grid[i][j] = Instantiate(grid0, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 1:
                        {
                            grid[i][j] = Instantiate(grid1, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 2:
                        {
                            grid[i][j] = Instantiate(grid2, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    case 3:
                        {
                            grid[i][j] = Instantiate(grid3, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                    default:
                        {
                            grid[i][j] = Instantiate(chasm, Vector3.right * i + Vector3.forward * j, Quaternion.identity).GetComponent<DungeonTile>();
                            break;
                        }
                }
            }
        }
    }
}
