using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breach : Projectile
{
    internal Hackable target;
    internal int breach;

    protected override void HitTarget()
    {
        target.Breach(breach);
    }

    internal void SetCourse(List<DungeonTile> path, Hackable target)
    {
        myTile = path[0];
        this.target = target;
        movePath = path;
    }
}
