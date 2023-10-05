using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RobotPlayer : BaseRobot
{
    [ClientRpc]
    public override void InitClientRpc()
    {
        Debug.Log("SETTING UP PLAYER ROBOT ON CLIENT");
        //GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetRobotSouthSprite();
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}
