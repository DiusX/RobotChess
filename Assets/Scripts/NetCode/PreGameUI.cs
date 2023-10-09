using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

//based on Unity Docs && tutorial from @CodeMonkey on YT
public class PreGameUI : MonoBehaviour
{
    public static PreGameUI Instance;

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        Instance = this;

        mainMenuButton.onClick.AddListener(() =>
        {
            RobotChessLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.LobbyScene);
        });
        readyButton.onClick.AddListener(() =>
        {
            onClickReady();
        });
    }

    private void Start()
    {
        Lobby lobby = RobotChessLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }

    private void onClickReady()
    {
        Debug.Log("OnCLickReady PreGameUI");
        GameManager.Instance.SetPlayerReadyServerRpc();
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => onClickUnready());
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "UNREADY";
    }
    private void onClickUnready()
    {
        Debug.Log("OnCLickUnready PreGameUI");
        GameManager.Instance.SetPlayerUnReadyServerRpc();
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(() => onClickReady());
        readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "READY";
    }

    [ClientRpc]
    public void GameStartedClientRpc()
    {
        gameObject.SetActive(false);
    }
}
