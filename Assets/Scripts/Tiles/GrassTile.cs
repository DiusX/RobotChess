using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrassTile : Tile
{ //From @Tarodev Youtube tutorials
    [SerializeField] private Color _baseColor, _offsetColor;

    public override void Init(int x, int y) {
        base.Init(x, y);
        var isOffset = (x + y) % 2 == 1;
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Init((int)vector.x, (int)vector.y);
        name = "(" + transform.position.x + "," + transform.position.y + ") GrassTile";
    }
}
