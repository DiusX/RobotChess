using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    public static NetworkManagerUI Instance;

    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        Instance = this;

        serverButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING SERVER");
            NetworkManager.Singleton.StartServer();
            switchConnectionButtons();
        });
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING HOST");
            NetworkManager.Singleton.StartHost();
            switchConnectionButtons();
        });
        clientButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING CLIENT");
            NetworkManager.Singleton.StartClient();
            switchConnectionButtons();
        });
        startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReadyServerRpc();
        });        
    }

    private void switchConnectionButtons()
    {
        serverButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void GameStartedClientRpc()
    {
        gameObject.SetActive(false);
    }
}
