using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuildingEnemy : BaseBuilding
{
    [ClientRpc]
    public override void InitClientRpc()
    {
        Debug.Log("SETTING UP ENEMY BUILDING ON CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetBuildingSprite();
        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
