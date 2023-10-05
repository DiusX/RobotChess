using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameOverController : NetworkBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _resultTextbox;
    public static GameOverController Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void ShowEndScreenClientRpc()
    {
        //get info from Unitmanager and set to label
    }
}
