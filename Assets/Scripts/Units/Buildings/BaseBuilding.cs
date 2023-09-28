using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseBuilding : BaseUnit
{
    private NetworkVariable<bool> _isShielded = new NetworkVariable<bool>(false); //Think about syncing this
    public bool IsShielded => _isShielded.Value;

    public override void GetShot(Faction faction)
    {
        if(Faction == faction)
        {
            _isShielded.Value = true;
        }
        else _isShielded.Value = false;
        _isShielded.SetDirty(true);
    }

    /*[ClientRpc]
    public override void GetShotClientRpc(Faction faction)
    {
        GetShot(faction);
    }*/

    public override void ClearShot()
    {
        _isShielded.Value = false;
        _isShielded.SetDirty(true);
    }

    /*[ClientRpc]
    public override void ClearShotClientRpc()
    {
        ClearShot();
    }*/
}
