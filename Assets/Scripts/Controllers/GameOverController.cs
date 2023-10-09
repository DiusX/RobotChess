using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : NetworkBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _resultTextbox;
    [SerializeField] private Button _returnToMainMenuButton;
    public static GameOverController Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void ShowEndScreenClientRpc(int scorePlayerOne, int scorePlayerTwo, Faction victorFaction)
    {
        Faction playerFaction = PlayerTurnManager.Instance.GetPlayerFaction(OwnerClientId);
        Debug.Log("Player Faction: " + playerFaction.ToString());

        string resultText;
        if (playerFaction == victorFaction)
        {
            resultText = "You Won!";
        }
        else
        {
            resultText = "You Lost!";
        }
        resultText += ("\nFinal Score: " + scorePlayerOne + " - " + scorePlayerTwo);

        _resultTextbox.text = resultText;
        _returnToMainMenuButton.onClick.AddListener(
            () => {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.HomeScene);
            });
        _container.SetActive(true);
    }
}
