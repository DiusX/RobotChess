using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerController : NetworkBehaviour
{
    [SerializeField] private GameObject _container;
    //[SerializeField] private TextMeshProUGUI _valueTextbox;
    [SerializeField] private Image _timerImage;
    
    public static CountdownTimerController Instance;
    private NetworkVariable<bool> _isRunning = new NetworkVariable<bool>(false);
    [SerializeField] private float _startTimer = 30f;
    private NetworkVariable<float> _timer = new NetworkVariable<float>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_isRunning.Value)
        {
            _timer.Value -= Time.deltaTime;
            if(_timer.Value < 0)
            {
                timerFinished();
            }
            else
            {
                //_valueTextbox.text = Mathf.Ceil(_timer).ToString();
                _timerImage.fillAmount = getCountdownTimerNormalized();
            }
        }
    }

    private void timerFinished()
    {
        _container.SetActive(false);
        _isRunning.Value = false;
        //Callback unitmanager destroy random building

    }

    public void StartTimer()
    {
        _timer.Value = _startTimer;
        _isRunning.Value = true;
        //_valueTextbox.text = Mathf.Ceil(_timer).ToString();
        _timerImage.fillAmount = getCountdownTimerNormalized();

        _container.SetActive(true);
    }

    private float getCountdownTimerNormalized()
    {
        return _timer.Value / _startTimer;
    }
}
