using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RobotEnemy : BaseRobot
{
    [ClientRpc]
    public override void InitClientRpc()
    {
        Debug.Log("SETTING UP ENEMY ROBOT ON CLIENT");
        //GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetEnemyRobotSprite();
    }
}
