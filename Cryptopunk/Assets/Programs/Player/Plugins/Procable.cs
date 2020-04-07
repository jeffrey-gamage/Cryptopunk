using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Procable : Plugin
{

    internal abstract void Proc(DungeonTile targetTile);
    internal abstract void Proc(Program targetProgram);
}
