using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RobotPlayer : BaseRobot
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Debug.Log("SETTING PLAYER ROBOT SPRITE CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetPlayerRobotSprite();
        transform.position = vector;
    }
}
