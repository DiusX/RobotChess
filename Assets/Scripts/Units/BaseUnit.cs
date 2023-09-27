using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseUnit : NetworkBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;

    [ClientRpc]
    public virtual void InitClientRpc(Vector2 vector)
    {
        Debug.Log("Base InitClientRpc called for unit");
    }

    public virtual void GetShot(Faction faction)
    {

    }

    /*[ClientRpc]
    public virtual void GetShotClientRpc(Faction faction)
    {
        Debug.Log("Base GetShotClientRpc called for unit");
    }*/

    public virtual void ClearShot()
    {

    }

   /* [ClientRpc]
    public virtual void ClearShotClientRpc()
    {
        Debug.Log("Base ClearShotClientRpc called for unit");
    }*/

    [ClientRpc]
    public virtual void AddUnitLocallyClientRpc()
    {
        UnitManager.Instance.AddUnitLocally(this);
    }    
}
