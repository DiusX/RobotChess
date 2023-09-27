
using System;
using Unity.Netcode;

public class BaseRobot : BaseUnit
{
    public UnitDirection direction;
    private NetworkVariable<bool> _isStunned = new NetworkVariable<bool>(false); //think about syncing this
    public bool IsStunned => _isStunned.Value;

    public override void GetShot(Faction faction)
    {
        if(Faction != faction)
        {
            _isStunned.Value = true;
        }
    }

    /*[ClientRpc]
    public override void GetShotClientRpc(Faction faction)
    {
        GetShot(faction);
    }*/

    public override void ClearShot()
    {
        _isStunned.Value = false;
    }

    /*[ClientRpc]
    public override void ClearShotClientRpc()
    {
        ClearShot();
    }*/


}
