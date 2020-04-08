using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicKnot : Procable
{
    [SerializeField] AudioClip stunEffect;

    internal override void Proc(DungeonTile targetTile)
    {
        //no valid action for targeting a tile. Do nothing and stay in proc targeting
    }

    internal override void Proc(Program targetProgram)
    {
        if (!targetProgram.IsControlledByPlayer())
        {
            AudioSource.PlayClipAtPoint(stunEffect, FindObjectOfType<AudioListener>().transform.position, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
            Stun(targetProgram);
            Stun(Program.selectedProgram);
            EndProc();
        }
    }

    private void Stun(Program targetProgram)
    {
        targetProgram.baseKeywords.Add("Stunned");
        targetProgram.movesLeft = 0;
        targetProgram.hasAttacked = true;
    }
}
