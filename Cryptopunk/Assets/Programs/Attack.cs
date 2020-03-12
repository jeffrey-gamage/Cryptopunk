using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Projectile
{
    internal Program target;
    internal int damage;

    protected override void HitTarget()
    {
        target.Damage(damage);
    }

    internal void SetCourse(List<DungeonTile> path, Program target)
    {
        myTile = path[0];
        this.target = target;
        movePath = path;
    }
}
