using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreboardManager : NetworkBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _valueTextbox;

    public static ScoreboardManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void UpdateScoreBoardClientRpc(int playerScore, int enemyScore)
    {
        _container.SetActive(true);
        _valueTextbox.text = ("Score:  (Blue) " + playerScore + " - (Red) " + enemyScore);
    }

    [ClientRpc]
    public void DisableScoreBoardClientRpc()
    {
        _container.SetActive(false);
    }
}
