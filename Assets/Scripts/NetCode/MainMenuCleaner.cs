using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleaner : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(RobotChessMultiplayer.Instance != null)
        {
            Destroy(RobotChessMultiplayer.Instance.gameObject);
        }

        if(RobotChessLobby.Instance != null)
        {
            Destroy(RobotChessLobby.Instance.gameObject);
        }
    }
}
