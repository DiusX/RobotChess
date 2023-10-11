using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerController : NetworkBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _valueTextbox;
    [SerializeField] private Image _timerImage;
    
    public static CountdownTimerController Instance;
    private NetworkVariable<bool> _isRunning = new NetworkVariable<bool>(false);
    [SerializeField] private float _startTimer = 60f;
    private NetworkVariable<float> _timer = new NetworkVariable<float>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (IsServer && _isRunning.Value)
        {
            _timer.Value -= Time.deltaTime;
            if(_timer.Value <= 0)
            {
                timerFinished();
            }
        }
        _timerImage.fillAmount = getCountdownTimerNormalized();
    }

    private void timerFinished()
    {
        StopTimer();
        InputController.Instance.DeactivateInputsClientRpc();
        RobotController.Instance.SkipTurnServerRpc();
    }

    public void StartTimer()
    {
        _timer.Value = _startTimer;
        _isRunning.Value = true;

        StartTimerOnClientRpc();
    }

    public void StopTimer()
    {
        _isRunning.Value = false;
        _container.SetActive(false);

        StopTimerOnClientRpc();
    }

    private float getCountdownTimerNormalized()
    {
        return _timer.Value / _startTimer;
    }

    [ClientRpc]
    public void StartTimerOnClientRpc()
    {
        Debug.LogWarning("TIMER ID: " + OwnerClientId);
        if (PlayerTurnManager.Instance.IsPlayerTurn(OwnerClientId))
        {
            _valueTextbox.text = "Your Turn:";
        }
        else _valueTextbox.text = "Enemy Turn:";
        _container.SetActive(true);
    }

    [ClientRpc]
    public void StopTimerOnClientRpc()
    {
        _container.SetActive(false);
    }
}
