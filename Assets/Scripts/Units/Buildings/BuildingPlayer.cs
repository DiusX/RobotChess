using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuildingPlayer : BaseBuilding
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Debug.Log("SETTING PLAYER BUILDING SPRITE CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetPlayerBuildingSprite();
        transform.position = vector;
    }
}
