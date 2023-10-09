using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

//based on Unity Docs && tutorial from @CodeMonkey on YT 
public class RobotChessMultiplayer : NetworkBehaviour
{
    public static RobotChessMultiplayer Instance;

    private string _playerName = "Player";

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

    public static bool playMultiplayer = true;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (!playMultiplayer)
        {
            // Singleplayer
            StartHost();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    public string GetPlayerName() { return _playerName; }

    public void SetPlayerName(string playerName) {  _playerName = playerName; }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
       
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.GameScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
       
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
