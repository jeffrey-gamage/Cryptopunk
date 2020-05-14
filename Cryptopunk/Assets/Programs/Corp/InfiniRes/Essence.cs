using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : Projectile
{
    internal Consumer target;

    protected override void HitTarget()
    {
        target.size++;
        target.basePower++;
        target.baseSpeed++;
        Destroy(gameObject);
    }

    internal void SetCourse(List<DungeonTile> path, Consumer target)
    {
        myTile = path[0];
        this.target = target;
        movePath = path;
    }
}
