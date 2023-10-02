using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuildingPlayer : BaseBuilding
{
    [ClientRpc]
    public override void InitClientRpc()
    {
        Debug.Log("SETTING UP PLAYER BUILDING ON CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetPlayerBuildingSprite();
    }
}
