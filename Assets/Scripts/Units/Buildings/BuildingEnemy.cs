using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BuildingEnemy : BaseBuilding
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Debug.Log("SETTING ENEMY BUILDING SPRITE CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetEnemyBuildingSprite();
        transform.position = vector;
    }
}
