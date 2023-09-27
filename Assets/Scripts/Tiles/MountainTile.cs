using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MountainTile : Tile
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Init((int)vector.x, (int)vector.y);
        name = "(" + transform.position.x + "," + transform.position.y + ") MountainTile";
    }
}
