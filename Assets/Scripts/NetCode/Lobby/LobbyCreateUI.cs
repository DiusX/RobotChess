using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Based on Unity docs and tutorial from @Code Monkey from YT
public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Awake()
    {
        createPublicButton.onClick.AddListener(() => {
            RobotChessLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() => {
            RobotChessLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        createPublicButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}