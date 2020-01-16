using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityNode : MonoBehaviour
{
    // Start is called before the first frame update
    private MeshRenderer myMeshRenderer;
    private SpriteRenderer myIcon;
    internal DungeonTile myTile;

    private void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myIcon = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if(myTile.isExplored)
        {
            myMeshRenderer.enabled = true;
            myIcon.enabled = true;
        }
    }
    internal void DeployReinforcement(GameObject reinforcement)
    {
        DungeonTile targetTile = SelectRandomAdjacentTile(myTile);
        EnemyProgram newEnemy = Instantiate(reinforcement).GetComponent<EnemyProgram>();
        if (targetTile != null && newEnemy != null)
        {
            DungeonManager.instance.DeploySecurity(newEnemy, targetTile);
            newEnemy.InitializeReinforcement();
        }
        else
        {
            Destroy(newEnemy.gameObject);
        }
    }

    private DungeonTile SelectRandomAdjacentTile(DungeonTile myTile)
    {
        List<DungeonTile> possibleTiles = new List<DungeonTile>();
        AddIfViable(possibleTiles, myTile.xCoord + 1, myTile.zCoord);
        AddIfViable(possibleTiles, myTile.xCoord - 1, myTile.zCoord);
        AddIfViable(possibleTiles, myTile.xCoord, myTile.zCoord + 1);
        AddIfViable(possibleTiles, myTile.xCoord, myTile.zCoord - 1);
        if (possibleTiles.Count > 0)
        {
            return possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count)];
        }
        return null;
    }

    private void AddIfViable(List<DungeonTile> possibleTiles, int xCoord, int zCoord)
    {
        if(DungeonManager.instance.grid.CanDeploySecurityHere(xCoord,zCoord))
        {
            possibleTiles.Add(DungeonManager.instance.grid.GetTile(xCoord,zCoord));
        }
    }
}
