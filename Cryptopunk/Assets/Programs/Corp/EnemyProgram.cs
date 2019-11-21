using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProgram : Program
{
    private MeshRenderer myRenderer;
    override internal void Start()
    {
        base.Start();
        myRenderer = GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    override internal void Update()
    {
        base.Update();
        if(myTile)
        {
            myRenderer.enabled = myTile.isVisible;
        }
    }
    internal override void OnStartTurn()
    {
        base.OnStartTurn();
        GetComponent<Hackable>().OnStartTurn();
    }
    override internal void Damage(int damageAmount)
    {
        GetComponent<Hackable>().Disrupt(damageAmount);
        base.Damage(damageAmount);
    }
}
