using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accumulator : EnemyProgram
{
    protected override void ExecuteAttack(Program target, List<DungeonTile> tempPath)
    {
        base.ExecuteAttack(target, tempPath);
        size++;
        basePower++;
    }
}
