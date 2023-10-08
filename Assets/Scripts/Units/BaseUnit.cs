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
    public virtual void InitClientRpc()
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
    public void SetUnitOnTileClientRpc(Vector2 tilePos)
    {
        Tile tile = TileManager.Instance.GetLocalPlayableTile(tilePos);
        if (tile == null) {
            Debug.LogError("MISSING TILE THAT UNIT WANTS TO BE SET ON: " + tilePos);
            return;
        }
        tile.SetUnit(this);
    }

    [ClientRpc]
    public void ClearUnitFromTileClientRpc()
    {
        OccupiedTile.ClearOccupiedUnit();
    }
}
