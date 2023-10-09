using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

//based on Unity Docs && tutorial from @CodeMonkey on YT
public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        RobotChessMultiplayer.Instance.OnFailedToJoinGame += RobotChessMultiplayer_OnFailedToJoinGame;
        RobotChessLobby.Instance.OnCreateLobbyStarted += RobotChessLobby_OnCreateLobbyStarted;
        RobotChessLobby.Instance.OnCreateLobbyFailed += RobotChessLobby_OnCreateLobbyFailed;
        RobotChessLobby.Instance.OnJoinStarted += RobotChessLobby_OnJoinStarted;
        RobotChessLobby.Instance.OnJoinFailed += RobotChessLobby_OnJoinFailed;
        RobotChessLobby.Instance.OnQuickJoinFailed += RobotChessLobby_OnQuickJoinFailed;

        Hide();
    }

    private void RobotChessLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void RobotChessLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
    }

    private void RobotChessLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void RobotChessLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void RobotChessLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void RobotChessMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        RobotChessMultiplayer.Instance.OnFailedToJoinGame -= RobotChessMultiplayer_OnFailedToJoinGame;
        RobotChessLobby.Instance.OnCreateLobbyStarted -= RobotChessLobby_OnCreateLobbyStarted;
        RobotChessLobby.Instance.OnCreateLobbyFailed -= RobotChessLobby_OnCreateLobbyFailed;
        RobotChessLobby.Instance.OnJoinStarted -= RobotChessLobby_OnJoinStarted;
        RobotChessLobby.Instance.OnJoinFailed -= RobotChessLobby_OnJoinFailed;
        RobotChessLobby.Instance.OnQuickJoinFailed -= RobotChessLobby_OnQuickJoinFailed;
    }
}
