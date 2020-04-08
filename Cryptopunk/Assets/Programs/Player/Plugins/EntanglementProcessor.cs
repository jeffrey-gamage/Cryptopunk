using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntanglementProcessor: Procable
{
    [SerializeField] GameObject teleportEffect;
    [SerializeField] AudioClip teleportSound;
    internal override void Proc(DungeonTile targetTile)
    {
        AudioSource.PlayClipAtPoint(teleportSound, FindObjectOfType<Camera>().transform.position, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
        if(!targetTile.isOccupied)
        {
            if(targetTile.IsVisible()&&((!targetTile.isBlocked)|| Program.selectedProgram.IsFlying()))
            {
                Instantiate(teleportEffect, Program.selectedProgram.transform.position, Quaternion.identity);
                Instantiate(teleportEffect, targetTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying(), false),Quaternion.identity);

                Program.selectedProgram.myTile.Vacate(Program.selectedProgram);
                Program.selectedProgram.myTile = targetTile;
                targetTile.Occupy(Program.selectedProgram);
                Program.selectedProgram.gameObject.transform.position = targetTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying(), false);

                DungeonManager.instance.mode = DungeonManager.Mode.Move;
            }
        }
    }

    internal override void Proc(Program targetProgram)
    {
        //swap places with target
        AudioSource.PlayClipAtPoint(teleportSound, FindObjectOfType<Camera>().transform.position, PlayerPrefs.GetFloat(Options.sfxVolumeKey));
        DungeonTile newTargetPosition = Program.selectedProgram.myTile;
        DungeonTile newSelectedPosition = targetProgram.myTile;

        Instantiate(teleportEffect, Program.selectedProgram.transform.position, Quaternion.identity);
        Instantiate(teleportEffect, targetProgram.transform.position, Quaternion.identity);

        targetProgram.myTile.Vacate(targetProgram);
        Program.selectedProgram.myTile.Vacate(Program.selectedProgram);

        targetProgram.myTile = newTargetPosition;
        Program.selectedProgram.myTile = newSelectedPosition;

        Program.selectedProgram.myTile.Occupy(Program.selectedProgram);
        targetProgram.myTile.Occupy(targetProgram);

        targetProgram.gameObject.transform.position = targetProgram.myTile.GetOccupyingCoordinates(targetProgram.IsFlying(), false);
        Program.selectedProgram.gameObject.transform.position = Program.selectedProgram.myTile.GetOccupyingCoordinates(Program.selectedProgram.IsFlying(), false);

        DungeonManager.instance.mode = DungeonManager.Mode.Move;
    }
}
