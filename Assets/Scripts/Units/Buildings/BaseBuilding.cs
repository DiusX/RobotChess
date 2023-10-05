using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseBuilding : BaseUnit
{

    [SerializeField] private GameObject _childGameObject;
    private NetworkVariable<bool> _isShielded = new NetworkVariable<bool>(false);
    public bool IsShielded => _isShielded.Value;

    private void Start()
    {
        _isShielded.OnValueChanged += onShieldedValueChanged;
    }
    private void onShieldedValueChanged(bool oldValue, bool newValue)
    {
        _childGameObject.SetActive(newValue);
        onClientRpc(newValue);
    }

    [ClientRpc]
    private void onClientRpc(bool newValue)
    {
        _childGameObject.SetActive(newValue);
    }

    public override void GetShot(Faction faction)
    {
        if (Faction == faction)
        {
            _isShielded.Value = true;
            SoundManager.Instance.PlayShieldUpSound(transform.position);
            AnimationManager.Instance.PlayParticleClientRpc(transform.position);
        }
        else
        {
            _isShielded.Value = false;
            SoundManager.Instance.PlayShieldDownSound(transform.position);
            AnimationManager.Instance.PlayParticleClientRpc(transform.position);
        }
        _isShielded.SetDirty(true);        
    }

    /*[ClientRpc]
    public override void GetShotClientRpc(Faction faction)
    {
        GetShot(faction);
    }*/

    public override void ClearShot()
    {
        _isShielded.Value = false;
        _isShielded.SetDirty(true);
    }

    /*[ClientRpc]
    public override void ClearShotClientRpc()
    {
        ClearShot();
    }*/

    
}
