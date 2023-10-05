using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnwalkableTile : Tile
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Init((int)vector.x, (int)vector.y);
        //_childGameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetUnwalkableTileSprite();
        name = "(" + transform.position.x + "," + transform.position.y + ") UnwalkableTile";
    }
}
