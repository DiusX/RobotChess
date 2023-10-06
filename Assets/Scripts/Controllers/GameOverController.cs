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
    public void ShowEndScreenClientRpc(int scorePlayer, int scoreEnemy)
    {
        /*string resultText = "You Won!";
        //get info from Unitmanager and set to label
        if (faction == Faction.Enemy) {
            resultText = "You Lost!";
        }
        resultText += ("\nFinal Score: {0}-{1}", scorePlayer, scoreEnemy);
        _resultTextbox.text = resultText;*/

        _container.SetActive(true);
        _resultTextbox.text = ("Final Score: " + scorePlayer + "-" + scoreEnemy);
        _returnToMainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }
}
