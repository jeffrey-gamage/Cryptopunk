using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : EnemyProgram
{
    //Enemy subclass for Infinity Resource Acquisition
    //can harvest allies to increase its own power
    [SerializeField] GameObject myEssence;
    
    [SerializeField] float consumeIntervalTime = 0.25f;
    private EnemyProgram consumeTarget;
    internal void AttemptConsume()
    {
        if(consumeTarget)
        {
            hasUsedAIAction = true;
            List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(consumeTarget.myTile,myTile, GetSight(), true);
            if (tempPath[tempPath.Count - 1] == myTile)
            {
                Consume(consumeTarget, tempPath);
            }
            else
            {
                Debug.LogError("consume target not visible");
            }
        }
        else
        {
            DungeonManager.instance.Resume();
        }
    }

    private void Consume(EnemyProgram consumeTarget, List<DungeonTile> tempPath)
    {
        DungeonManager.instance.Wait();
        Essence newEssence = Instantiate(myEssence, consumeTarget.transform.position, Quaternion.identity).GetComponent<Essence>();
        newEssence.Harvest(consumeTarget);
        newEssence.SetCourse(tempPath, this);
        Invoke("AttemptConsume", consumeIntervalTime);
    }

    internal void SelectConsumeTarget()
    {
        consumeTarget = null;
        foreach (EnemyProgram program in DungeonManager.instance.GetAIControlledPrograms())
        {
            if (CanSee(program))
            {
                if (program!=this&&(!consumeTarget || (DungeonManager.instance.grid.TileDistance(program.myTile, myTile) < DungeonManager.instance.grid.TileDistance(consumeTarget.myTile, myTile))))
                {
                    consumeTarget = program;
                    Debug.Log("Consume Target selected");
                }
            }
        }
    }
}
