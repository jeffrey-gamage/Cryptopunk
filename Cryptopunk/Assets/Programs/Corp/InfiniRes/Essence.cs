using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : Projectile
{
    internal Consumer target;
    private int power =0;
    private int speed=0;
    private int size=0;

    protected override void HitTarget()
    {
        target.size+=size;
        target.basePower+=power;
        target.baseSpeed+=speed;
        Destroy(gameObject);
    }

    internal void SetCourse(List<DungeonTile> path, Consumer target)
    {
        myTile = path[0];
        this.target = target;
        movePath = path;
    }

    internal void Harvest(EnemyProgram consumeTarget)
    {
        if(consumeTarget.basePower>0&&consumeTarget.basePower>consumeTarget.baseSpeed)
        {
            consumeTarget.basePower--;
            power++;
        }
        else if(consumeTarget.baseSpeed>0)
        {
            consumeTarget.baseSpeed--;
            speed++;
        }
        consumeTarget.Damage(1);
        size++;
    }
}
