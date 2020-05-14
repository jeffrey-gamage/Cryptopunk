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

    internal override void AttemptAttack(Program target)
    {
        if (!hasUsedAction && GetPower() > 0)
        {
            List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(myTile, target.myTile, GetRange(), true);
            if (tempPath[tempPath.Count - 1] == target.myTile)
            {
                ExecuteAttack(target, tempPath);
            }
            else
            {
                Debug.Log("Target out of range");
                if(myState==State.Attack)
                {
                    SelectConsumeTarget();
                    AttemptConsume();
                }
            }
        }
        else
        {
            Debug.Log(gameObject.name + " should not be able to attack");
        }
    }

    private void AttemptConsume()
    {
        if(consumeTarget)
        {
            List<DungeonTile> tempPath = DungeonManager.instance.grid.FindPath(consumeTarget.myTile,myTile, GetSight(), true);
            if (tempPath[tempPath.Count - 1] == consumeTarget.myTile)
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
        consumeTarget.Damage(1);
        Essence newEssence = Instantiate(myEssence, gameObject.transform.position, Quaternion.identity).GetComponent<Essence>();
        newEssence.SetCourse(tempPath, this);
        Invoke("AttemptConsume", consumeIntervalTime);
    }

    private void SelectConsumeTarget()
    {
        consumeTarget = null;
        foreach (EnemyProgram program in DungeonManager.instance.GetAIControlledPrograms())
        {
            if (CanSee(program))
            {
                if (!consumeTarget || (DungeonManager.instance.grid.TileDistance(program.myTile, myTile) < DungeonManager.instance.grid.TileDistance(consumeTarget.myTile, myTile)))
                {
                    consumeTarget = program;
                    Debug.Log("Consume Target selected");
                }
            }
        }
    }
}
