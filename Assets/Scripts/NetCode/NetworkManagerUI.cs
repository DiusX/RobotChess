using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING SERVER");
            NetworkManager.Singleton.StartServer();            
        });
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING HOST");
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() =>
        {
            Debug.Log("STARTING CLIENT");
            NetworkManager.Singleton.StartClient();
        });
    }
}
