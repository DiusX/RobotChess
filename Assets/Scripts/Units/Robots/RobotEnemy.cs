using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RobotEnemy : BaseRobot
{
    [ClientRpc]
    public override void InitClientRpc(Vector2 vector)
    {
        Debug.Log("SETTING ENEMY ROBOT SPRITE CLIENT");
        GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetEnemyRobotSprite();
        transform.position = vector;
    }
}
