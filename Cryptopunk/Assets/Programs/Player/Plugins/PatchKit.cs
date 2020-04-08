using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchKit : Procable
{
    [SerializeField] GameObject healParticleEffect;
    [SerializeField] AudioClip healSoundEffect;
    [SerializeField] int healAmount;

    internal override void Proc(DungeonTile targetTile)
    {
        //no valid action for targeting a tile. Do nothing and stay in proc targeting
    }

    internal override void Proc(Program targetProgram)
    {
        if (healSoundEffect)
        {
            AudioSource.PlayClipAtPoint(healSoundEffect, FindObjectOfType<Camera>().transform.position, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
        }
        if (healParticleEffect)
        {
            Instantiate(healParticleEffect, targetProgram.transform.position, Quaternion.identity);
        }
        targetProgram.size = Mathf.Min(targetProgram.size + healAmount, targetProgram.GetSize());
        EndProc();
    }
}
