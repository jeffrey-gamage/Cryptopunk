using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBridge : Hackable
{

    internal List<SwitchTile> controlledTilesEnabled;
    internal List<SwitchTile> controlledTilesDisabled;


    internal override void Activate()
    {
        base.Activate();
        MoveTiles();
    }

    internal override void Deactivate(bool isBreach)
    {
        base.Deactivate(isBreach);
        MoveTiles();
    }

    private void MoveTiles()
    {
        foreach(SwitchTile tile in controlledTilesEnabled)
        {
            if(isEnabled)
            {
                tile.Switch(true);
            }
            else
            {
                tile.Switch(false);
            }
        }
        foreach(SwitchTile tile in controlledTilesDisabled)
        {
            if(isEnabled)
            {
                tile.Switch(false);
            }
            else
            {
                tile.Switch(true);
            }
        }
    }
    internal class SwitchTileAssignment
    {

        internal Vector3Int terminalLocation;
        internal List<Vector3Int> controlLocations;
        private bool terminalLocationIsSet = false;

        internal SwitchTileAssignment()
        {
            controlLocations = new List<Vector3Int>();
        }

        public void AddLocation(Vector3Int newLocation)
        {
            if (!terminalLocationIsSet)
            {
                terminalLocationIsSet = true;
                terminalLocation = newLocation;
            }
            else
            {
                controlLocations.Add(newLocation);
            }
        }

        internal void Translate(Vector3Int translationVector)
        {
            terminalLocation += translationVector;
            if (controlLocations.Count > 0)
            {
                for (int i = 0; i < controlLocations.Count; i++)
                {
                    controlLocations[i] += translationVector;
                }
            }
        }
    }
}
